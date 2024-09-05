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

public class RunPrediction : IComponent
{
    protected InfoTimeComponent InternalComponent { get; set; }
    public RunPredictionSettings Settings { get; set; }
    private SplitTimeFormatter Formatter { get; set; }
    private string PreviousInformationName { get; set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    public IDictionary<string, Action> ContextMenuControls => null;

    public RunPrediction(LiveSplitState state)
    {
        Settings = new RunPredictionSettings()
        {
            CurrentState = state
        };
        Formatter = new SplitTimeFormatter(Settings.Accuracy);
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

        InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;
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

    public string ComponentName => GetDisplayedName(Settings.Comparison);

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

    protected string GetDisplayedName(string comparison)
    {
        return comparison switch
        {
            "Current Comparison" => "Current Pace",
            Run.PersonalBestComparisonName => "Current Pace",
            BestSegmentsComparisonGenerator.ComparisonName => "Best Possible Time",
            WorstSegmentsComparisonGenerator.ComparisonName => "Worst Possible Time",
            AverageSegmentsComparisonGenerator.ComparisonName => "Predicted Time",
            _ => "Current Pace (" + CompositeComparisons.GetShortComparisonName(comparison) + ")",
        };
    }

    protected void SetAlternateText(string comparison)
    {
        InternalComponent.AlternateNameText = comparison switch
        {
            "Current Comparison" => new[]
                            {
                    "Cur. Pace",
                    "Pace"
                },
            Run.PersonalBestComparisonName =>
                [
                    "Cur. Pace",
                    "Pace"
                ],
            BestSegmentsComparisonGenerator.ComparisonName =>
                [
                    "Best Poss. Time",
                    "Best Time",
                    "BPT"
                ],
            WorstSegmentsComparisonGenerator.ComparisonName =>
                [
                    "Worst Poss. Time",
                    "Worst Time"
                ],
            AverageSegmentsComparisonGenerator.ComparisonName =>
                [
                    "Pred. Time",
                ],
            _ =>
                [
                    "Current Pace",
                    "Cur. Pace",
                    "Pace"
                ],
        };
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        string comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
        if (!state.Run.Comparisons.Contains(comparison))
        {
            comparison = state.CurrentComparison;
        }

        InternalComponent.InformationName = InternalComponent.LongestString = GetDisplayedName(comparison);

        if (InternalComponent.InformationName != PreviousInformationName)
        {
            SetAlternateText(comparison);
            PreviousInformationName = InternalComponent.InformationName;
        }

        if (InternalComponent.InformationName.StartsWith("Current Pace") && state.CurrentPhase == TimerPhase.NotRunning)
        {
            InternalComponent.TimeValue = null;
        }
        else if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
        {
            TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, state.CurrentTimingMethod) ?? TimeSpan.Zero;
            TimeSpan? liveDelta = state.CurrentTime[state.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][state.CurrentTimingMethod];
            if (liveDelta > delta)
            {
                delta = liveDelta;
            }

            InternalComponent.TimeValue = delta + state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
        }
        else if (state.CurrentPhase == TimerPhase.Ended)
        {
            InternalComponent.TimeValue = state.Run.Last().SplitTime[state.CurrentTimingMethod];
        }
        else
        {
            InternalComponent.TimeValue = state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
        }

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
