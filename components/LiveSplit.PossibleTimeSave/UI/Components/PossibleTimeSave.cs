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

public class PossibleTimeSave : IComponent
{
    protected InfoTimeComponent InternalComponent { get; set; }
    public PossibleTimeSaveSettings Settings { get; set; }
    private PossibleTimeSaveFormatter Formatter { get; set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    public IDictionary<string, Action> ContextMenuControls => null;

    public PossibleTimeSave(LiveSplitState state)
    {
        Formatter = new PossibleTimeSaveFormatter();
        InternalComponent = new InfoTimeComponent(null, null, Formatter);
        Settings = new PossibleTimeSaveSettings()
        {
            CurrentState = state
        };
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

    public string ComponentName
    => (Settings.TotalTimeSave ? "Total " : "") + "Possible Time Save"
        + (Settings.Comparison == "Current Comparison"
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

    public TimeSpan? GetPossibleTimeSave(LiveSplitState state, ISegment segment, string comparison, bool live = false)
    {
        int splitIndex = state.Run.IndexOf(segment);
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

        TimeSpan? time = segment.Comparisons[comparison][state.CurrentTimingMethod] - prevTime - bestSegments;

        if (live && splitIndex == state.CurrentSplitIndex)
        {
            TimeSpan? segmentDelta = TimeSpan.Zero - LiveSplitStateHelper.GetLiveSegmentDelta(state, state.Run.IndexOf(segment), comparison, state.CurrentTimingMethod);
            if (segmentDelta < time)
            {
                time = segmentDelta;
            }
        }

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
        string componentName = (Settings.TotalTimeSave ? "Total " : "") + "Possible Time Save" + (Settings.Comparison == "Current Comparison" ? "" : " (" + comparisonName + ")");

        if (InternalComponent.InformationName != componentName)
        {
            InternalComponent.AlternateNameText.Clear();
            if (componentName.Contains("Total"))
            {
                InternalComponent.AlternateNameText.Add("Total Possible Time Save");
            }

            InternalComponent.AlternateNameText.Add("Possible Time Save");
            InternalComponent.AlternateNameText.Add("Poss. Time Save");
            InternalComponent.AlternateNameText.Add("Time Save");
        }

        InternalComponent.LongestString = componentName;
        InternalComponent.InformationName = componentName;

        if (Settings.TotalTimeSave)
        {
            if (state.CurrentPhase == TimerPhase.Ended)
            {
                InternalComponent.TimeValue = TimeSpan.Zero;
            }
            else
            {
                TimeSpan? totalPossibleTimeSave = state.Run
                    .Skip(state.CurrentSplitIndex)
                    .Select(x => GetPossibleTimeSave(state, x, comparison, true))
                    .Where(x => x.HasValue)
                    .Aggregate((TimeSpan?)TimeSpan.Zero, (a, b) => a + b);

                InternalComponent.TimeValue = totalPossibleTimeSave;
            }
        }
        else
        {
            if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
            {
                InternalComponent.TimeValue = GetPossibleTimeSave(state, state.CurrentSplit, comparison);
            }
            else
            {
                InternalComponent.TimeValue = null;
            }
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
