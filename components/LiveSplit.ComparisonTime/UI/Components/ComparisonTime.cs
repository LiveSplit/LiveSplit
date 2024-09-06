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

public class ComparisonTime : IComponent
{
    protected InfoTimeComponent InternalComponent { get; set; }
    public ComparisonTimeSettings Settings { get; set; }
    private SplitTimeFormatter Formatter { get; set; }
    private string PreviousInformationName { get; set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;
    public float VerticalHeight => InternalComponent.VerticalHeight;
    public float MinimumWidth => InternalComponent.MinimumWidth;
    public float HorizontalWidth => InternalComponent.HorizontalWidth;
    public float MinimumHeight => InternalComponent.MinimumHeight;

    public string ComponentName => GetComponentName();

    public IDictionary<string, Action> ContextMenuControls => null;

    public ComparisonTime(LiveSplitState state)
    {
        Settings = new ComparisonTimeSettings()
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

    protected void SetAlternateText(string comparison)
    {
        switch (comparison)
        {
            case Run.PersonalBestComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    "PB",
                };
                break;
            case AverageSegmentsComparisonGenerator.ComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    AverageSegmentsComparisonGenerator.ShortComparisonName,
                };
                break;
            case BestSegmentsComparisonGenerator.ComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    BestSegmentsComparisonGenerator.ShortComparisonName,
                };
                break;
            case LatestRunComparisonGenerator.ComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    LatestRunComparisonGenerator.ShortComparisonName,
                };
                break;
            case MedianSegmentsComparisonGenerator.ComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    MedianSegmentsComparisonGenerator.ShortComparisonName,
                };
                break;
            case PercentileComparisonGenerator.ComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    PercentileComparisonGenerator.ShortComparisonName,
                };
                break;
            case WorstSegmentsComparisonGenerator.ComparisonName:
                InternalComponent.AlternateNameText = new[]
                {
                    WorstSegmentsComparisonGenerator.ShortComparisonName,
                };
                break;
            default:
                break;
        }
    }

    protected string GetNameValue(string comparison)
    {
        if (Settings.TimingMethod != "Current Timing Method")
        {
            return $"{comparison} ({Settings.TimingMethod})";
        }

        return comparison;
    }

    protected TimeSpan? GetTimeValue(LiveSplitState state, string comparison, TimingMethod timingMethod)
    {
        if (Settings.Type == TimeType.FinalTime)
        {
            return state.Run.Last().Comparisons[comparison][timingMethod];
        }

        if (Settings.Type == TimeType.SplitTime)
        {
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                return null;
            }

            if (state.CurrentPhase == TimerPhase.Ended)
            {
                return state.Run.Last().Comparisons[comparison][timingMethod];
            }

            return state.Run[state.CurrentSplitIndex].Comparisons[comparison][timingMethod];
        }
        // Settings.Type == TimeType.SegmentTime
        if (state.CurrentPhase == TimerPhase.NotRunning)
        {
            return null;
        }

        TimeSpan? currentSplitComparisonTime;
        if (state.CurrentPhase == TimerPhase.Ended)
        {
            currentSplitComparisonTime = state.Run.Last().Comparisons[comparison][timingMethod];
        }
        else
        {
            currentSplitComparisonTime = state.Run[state.CurrentSplitIndex].Comparisons[comparison][timingMethod];
        }

        int previousSplitIndex = state.CurrentSplitIndex - 1;
        if (state.CurrentPhase == TimerPhase.Ended)
        {
            previousSplitIndex = state.Run.Count - 2;
        }

        TimeSpan? previousSplitComparisonTime;
        if (previousSplitIndex < 0)
        {
            previousSplitComparisonTime = TimeSpan.Zero;
        }
        else
        {
            previousSplitComparisonTime = state.Run[previousSplitIndex].Comparisons[comparison][timingMethod];
        }

        return currentSplitComparisonTime - previousSplitComparisonTime;
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        string comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
        if (!state.Run.Comparisons.Contains(comparison))
        {
            comparison = state.CurrentComparison;
        }

        TimingMethod timingMethod = state.CurrentTimingMethod;
        if (Settings.TimingMethod == "Real Time")
        {
            timingMethod = TimingMethod.RealTime;
        }
        else if (Settings.TimingMethod == "Game Time")
        {
            timingMethod = TimingMethod.GameTime;
        }

        InternalComponent.InformationName = InternalComponent.LongestString = GetNameValue(comparison);
        if (InternalComponent.InformationName != PreviousInformationName)
        {
            SetAlternateText(comparison);
            PreviousInformationName = InternalComponent.InformationName;
        }

        InternalComponent.TimeValue = GetTimeValue(state, comparison, timingMethod);
        InternalComponent.Update(invalidator, state, width, height, mode);
    }

    protected string GetComponentName()
    {
        bool isComparisonOverride = Settings.Comparison != "Current Comparison";
        bool isTimingMethodOverride = Settings.TimingMethod != "Current Timing Method";

        if (isComparisonOverride && isTimingMethodOverride)
        {
            return $"Comparison Time ({Settings.Comparison}, {Settings.TimingMethod})";
        }

        if (isComparisonOverride)
        {
            return $"Comparison Time ({Settings.Comparison})";
        }

        if (isTimingMethodOverride)
        {
            return $"Comparison Time ({Settings.TimingMethod})";
        }

        return "Comparison Time";
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
