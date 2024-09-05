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

public class PreviousSegment : IComponent
{
    protected InfoTimeComponent InternalComponent { get; set; }
    public PreviousSegmentSettings Settings { get; set; }

    protected DeltaTimeFormatter DeltaFormatter { get; set; }
    protected PossibleTimeSaveFormatter TimeSaveFormatter { get; set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    private string previousNameText { get; set; }

    public IDictionary<string, Action> ContextMenuControls => null;

    public PreviousSegment(LiveSplitState state)
    {
        DeltaFormatter = new DeltaTimeFormatter();
        TimeSaveFormatter = new PossibleTimeSaveFormatter();
        Settings = new PreviousSegmentSettings()
        {
            CurrentState = state
        };
        InternalComponent = new InfoTimeComponent(null, null, DeltaFormatter);
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
        => "Previous Segment" + (Settings.Comparison == "Current Comparison"
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

    public TimeSpan? GetPossibleTimeSave(LiveSplitState state, int splitIndex, string comparison)
    {
        TimeSpan prevTime = TimeSpan.Zero;
        TimeSpan? bestSegments = state.Run[splitIndex].BestSegmentTime[state.CurrentTimingMethod];

        while (splitIndex > 0 && bestSegments != null)
        {
            TimeSpan? splitTime = state.Run[splitIndex - 1].Comparisons[comparison][state.CurrentTimingMethod];
            if (splitTime != null)
            {
                prevTime = splitTime.Value;
                break;
            }
            else
            {
                splitIndex--;
                bestSegments += state.Run[splitIndex].BestSegmentTime[state.CurrentTimingMethod];
            }
        }

        TimeSpan? time = state.Run[splitIndex].Comparisons[comparison][state.CurrentTimingMethod] - prevTime - bestSegments;

        if (time < TimeSpan.Zero)
        {
            time = TimeSpan.Zero;
        }

        return time;
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        string comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
        if (!state.Run.Comparisons.Contains(comparison))
        {
            comparison = state.CurrentComparison;
        }

        string comparisonName = CompositeComparisons.GetShortComparisonName(comparison);
        string componentName = "Previous Segment" + (Settings.Comparison == "Current Comparison" ? "" : " (" + comparisonName + ")");

        InternalComponent.LongestString = componentName;
        InternalComponent.InformationName = componentName;

        DeltaFormatter.Accuracy = Settings.DeltaAccuracy;
        DeltaFormatter.DropDecimals = Settings.DropDecimals;
        TimeSaveFormatter.Accuracy = Settings.TimeSaveAccuracy;

        TimeSpan? timeChange = null;
        TimeSpan? timeSave = null;
        TimeSpan? liveSegment = LiveSplitStateHelper.CheckLiveDelta(state, false, comparison, state.CurrentTimingMethod);
        if (state.CurrentPhase != TimerPhase.NotRunning)
        {
            if (liveSegment != null)
            {
                timeChange = liveSegment;
                timeSave = GetPossibleTimeSave(state, state.CurrentSplitIndex, comparison);
                InternalComponent.InformationName = "Live Segment" + (Settings.Comparison == "Current Comparison" ? "" : " (" + comparisonName + ")");
            }
            else if (state.CurrentSplitIndex > 0)
            {
                timeChange = LiveSplitStateHelper.GetPreviousSegmentDelta(state, state.CurrentSplitIndex - 1, comparison, state.CurrentTimingMethod);
                timeSave = GetPossibleTimeSave(state, state.CurrentSplitIndex - 1, comparison);
            }

            if (timeChange != null)
            {
                if (liveSegment != null)
                {
                    InternalComponent.ValueLabel.ForeColor = LiveSplitStateHelper.GetSplitColor(state, timeChange, state.CurrentSplitIndex, false, false, comparison, state.CurrentTimingMethod).Value;
                }
                else
                {
                    InternalComponent.ValueLabel.ForeColor = LiveSplitStateHelper.GetSplitColor(state, timeChange.Value, state.CurrentSplitIndex - 1, false, true, comparison, state.CurrentTimingMethod).Value;
                }
            }
            else
            {
                Color? color = LiveSplitStateHelper.GetSplitColor(state, null, state.CurrentSplitIndex - 1, true, true, comparison, state.CurrentTimingMethod);
                if (color == null)
                {
                    color = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
                }

                InternalComponent.ValueLabel.ForeColor = color.Value;
            }
        }
        else
        {
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        }

        if (InternalComponent.InformationName != previousNameText)
        {
            InternalComponent.AlternateNameText.Clear();
            if (liveSegment != null)
            {
                InternalComponent.AlternateNameText.Add("Live Segment");
                InternalComponent.AlternateNameText.Add("Live Seg.");
            }
            else
            {
                InternalComponent.AlternateNameText.Add("Previous Segment");
                InternalComponent.AlternateNameText.Add("Prev. Segment");
                InternalComponent.AlternateNameText.Add("Prev. Seg.");
            }

            previousNameText = InternalComponent.InformationName;
        }

        InternalComponent.InformationValue = DeltaFormatter.Format(timeChange)
            + (Settings.ShowPossibleTimeSave ? " / " + TimeSaveFormatter.Format(timeSave) : "");

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
