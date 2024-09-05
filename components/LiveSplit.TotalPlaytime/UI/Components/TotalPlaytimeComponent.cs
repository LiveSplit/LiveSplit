using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public class TotalPlaytimeComponent : IComponent
{
    protected ITimeFormatter HoursTimeFormatter { get; set; }
    protected ITimeFormatter DaysTimeFormatter { get; set; }
    protected InfoTimeComponent InternalComponent { get; set; }
    protected TotalPlaytimeSettings Settings { get; set; }

    protected TimerPhase LastPhase { get; set; }
    protected int LastAttemptCount { get; set; }
    protected IRun LastRun { get; set; }

    public string ComponentName => "Total Playtime";

    public IDictionary<string, Action> ContextMenuControls => null;

    public float HorizontalWidth => InternalComponent.HorizontalWidth;

    public float VerticalHeight => InternalComponent.VerticalHeight;

    public float MinimumHeight => InternalComponent.MinimumHeight;

    public float MinimumWidth => InternalComponent.MinimumWidth;

    public float PaddingBottom => InternalComponent.PaddingBottom;

    public float PaddingLeft => InternalComponent.PaddingLeft;

    public float PaddingRight => InternalComponent.PaddingRight;

    public float PaddingTop => InternalComponent.PaddingTop;

    public TotalPlaytimeComponent(LiveSplitState state)
    {
        HoursTimeFormatter = new RegularTimeFormatter();
        DaysTimeFormatter = new DaysTimeFormatter();
        InternalComponent = new InfoTimeComponent("Total Playtime", TimeSpan.Zero, DaysTimeFormatter);
        Settings = new TotalPlaytimeSettings()
        {
            CurrentState = state
        };
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

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawBackground(g, state, HorizontalWidth, height);

        InternalComponent.NameLabel.HasShadow
            = InternalComponent.ValueLabel.HasShadow
            = state.LayoutSettings.DropShadows;

        InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

        InternalComponent.DrawHorizontal(g, state, height, clipRegion);
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawBackground(g, state, width, VerticalHeight);

        InternalComponent.DisplayTwoRows = Settings.Display2Rows;

        InternalComponent.NameLabel.HasShadow
            = InternalComponent.ValueLabel.HasShadow
            = state.LayoutSettings.DropShadows;

        InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

        InternalComponent.DrawVertical(g, state, width, clipRegion);
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public Control GetSettingsControl(LayoutMode mode)
    {
        Settings.Mode = mode;
        return Settings;
    }

    public void SetSettings(XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    public TimeSpan CalculateTotalPlaytime(LiveSplitState state)
    {
        TimeSpan totalPlaytime = TimeSpan.Zero;

        foreach (Attempt attempt in state.Run.AttemptHistory)
        {
            TimeSpan? duration = attempt.Duration;

            if (duration.HasValue)
            {
                //Either >= 1.6.0 or a finished run
                totalPlaytime += duration.Value - (attempt.PauseTime ?? TimeSpan.Zero);
            }
            else
            {
                //Must be < 1.6.0 and a reset
                //Calculate the sum of the segments for that run

                foreach (ISegment segment in state.Run)
                {
                    if (segment.SegmentHistory.TryGetValue(attempt.Index, out Time segmentHistoryElement) && segmentHistoryElement.RealTime.HasValue)
                    {
                        totalPlaytime += segmentHistoryElement.RealTime.Value;
                    }
                }
            }
        }

        totalPlaytime += state.CurrentAttemptDuration - (state.PauseTime ?? TimeSpan.Zero);

        return totalPlaytime;
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        InternalComponent.Formatter = Settings.ShowTotalHours ? HoursTimeFormatter : DaysTimeFormatter;

        if (LastAttemptCount != state.Run.AttemptHistory.Count
            || LastPhase != state.CurrentPhase
            || LastRun != state.Run
            || state.CurrentPhase == TimerPhase.Running
            || state.CurrentPhase == TimerPhase.Paused)
        {
            InternalComponent.TimeValue = CalculateTotalPlaytime(state);

            LastAttemptCount = state.Run.AttemptHistory.Count;
            LastPhase = state.CurrentPhase;
            LastRun = state.Run;
        }

        InternalComponent.Update(invalidator, state, width, height, mode);
    }

    public void Dispose()
    {
        InternalComponent.Dispose();
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
