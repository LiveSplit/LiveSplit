using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public class DeltaComponent : IComponent
{
    protected InfoTimeComponent InternalComponent { get; set; }
    public DeltaSettings Settings { get; set; }
    private GeneralTimeFormatter Formatter { get; set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    public IDictionary<string, Action> ContextMenuControls => null;

    public DeltaComponent(LiveSplitState state)
    {
        Settings = new DeltaSettings()
        {
            CurrentState = state
        };
        Formatter = new GeneralTimeFormatter()
        {
            NullFormat = NullFormat.Dash,
            Accuracy = Settings.Accuracy,
            DropDecimals = Settings.DropDecimals,
        };
        InternalComponent = new InfoTimeComponent(null, null, Formatter);
        state.ComparisonRenamed += state_ComparisonRenamed;
    }

    private void state_ComparisonRenamed(object sender, EventArgs e)
    {
        var args = (RenameEventArgs)e;
        if (Settings.Comparison == args.OldName)
        {
            Settings.Comparison = args.NewName;
            ((LiveSplitState)sender).Layout.HasChanged = true;
        }
    }

    private void PrepareDraw(LiveSplitState state)
    {
        InternalComponent.DisplayTwoRows = Settings.Display2Rows;

        InternalComponent.NameLabel.HasShadow
            = InternalComponent.ValueLabel.HasShadow
            = state.LayoutSettings.DropShadows;

        Formatter.Accuracy = Settings.Accuracy;
        Formatter.DropDecimals = Settings.DropDecimals;

        InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
    }

    private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
    {
        if (Settings.BackgroundColor.A > 0
            || (Settings.BackgroundGradient != GradientType.Plain
            && Settings.BackgroundColor2.A > 0))
        {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.BackgroundGradient == GradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        Settings.BackgroundColor,
                        Settings.BackgroundGradient == GradientType.Plain
                        ? Settings.BackgroundColor
                        : Settings.BackgroundColor2);
            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawBackground(g, state, width, VerticalHeight);
        PrepareDraw(state);
        InternalComponent.DrawVertical(g, state, width, clipRegion);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawBackground(g, state, HorizontalWidth, height);
        PrepareDraw(state);
        InternalComponent.DrawHorizontal(g, state, height, clipRegion);
    }

    public float VerticalHeight => InternalComponent.VerticalHeight;

    public float MinimumWidth => InternalComponent.MinimumWidth;

    public float HorizontalWidth => InternalComponent.HorizontalWidth;

    public float MinimumHeight => InternalComponent.MinimumHeight;

    public string ComponentName
        => "Delta" + (Settings.Comparison == "Current Comparison"
            ? ""
            : " (" + CompositeComparisons.GetShortComparisonName(Settings.Comparison) + ")");

    public Control GetSettingsControl(LayoutMode mode)
    {
        Settings.Mode = mode;
        return Settings;
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        string comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
        if (!state.Run.Comparisons.Contains(comparison))
        {
            comparison = state.CurrentComparison;
        }

        string comparisonName = comparison.StartsWith("[Race] ") ? comparison[7..] : comparison;

        bool useLiveDelta = false;
        if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
        {
            TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, state.CurrentTimingMethod);
            TimeSpan? liveDelta = state.CurrentTime[state.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][state.CurrentTimingMethod];
            if (liveDelta > delta || (delta == null && liveDelta > TimeSpan.Zero))
            {
                delta = liveDelta;
                useLiveDelta = true;
            }

            InternalComponent.TimeValue = delta;
        }
        else if (state.CurrentPhase == TimerPhase.Ended)
        {
            InternalComponent.TimeValue = state.Run.Last().SplitTime[state.CurrentTimingMethod] - state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
        }
        else
        {
            InternalComponent.TimeValue = null;
        }

        string text = comparisonName;
        if (Settings.OverrideText)
        {
            InternalComponent.AlternateNameText.Clear();
            if (Settings.DifferentialText)
            {
                text = InternalComponent.TimeValue < TimeSpan.Zero ? Settings.CustomTextAhead : Settings.CustomText;
                InternalComponent.LongestString = Settings.CustomText.Length < Settings.CustomTextAhead.Length ? Settings.CustomTextAhead : Settings.CustomText;
            }
            else
            {
                text = Settings.CustomText;
                InternalComponent.LongestString = text;
            }
        }
        else
        {
            InternalComponent.LongestString = text;
            if (InternalComponent.InformationName != text)
            {
                InternalComponent.AlternateNameText.Clear();
                InternalComponent.AlternateNameText.Add(CompositeComparisons.GetShortComparisonName(comparison));
            }
        }

        InternalComponent.InformationName = text;

        Color? color = LiveSplitStateHelper.GetSplitColor(state, InternalComponent.TimeValue, state.CurrentSplitIndex - (useLiveDelta ? 0 : 1), true, false, comparison, state.CurrentTimingMethod);
        if (color == null)
        {
            color = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        }

        InternalComponent.ValueLabel.ForeColor = color.Value;

        InternalComponent.Update(invalidator, state, width, height, mode);
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
