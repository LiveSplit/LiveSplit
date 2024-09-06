using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public class SumOfBestComponent : IComponent
{
    protected InfoTimeComponent InternalComponent { get; set; }
    public SumOfBestSettings Settings { get; set; }
    protected LiveSplitState CurrentState { get; set; }

    protected bool PreviousCalculationMode { get; set; }
    protected TimingMethod PreviousTimingMethod { get; set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    public TimeSpan? SumOfBestValue { get; set; }

    private SplitTimeFormatter Formatter { get; set; }

    public IDictionary<string, Action> ContextMenuControls => null;

    public SumOfBestComponent(LiveSplitState state)
    {
        Formatter = new SplitTimeFormatter();
        InternalComponent = new InfoTimeComponent("Sum of Best Segments", null, Formatter)
        {
            AlternateNameText = new string[]
            {
                "Sum of Best",
                "SoB"
            }
        };
        Settings = new SumOfBestSettings();
        state.OnSplit += state_OnSplit;
        state.OnUndoSplit += state_OnUndoSplit;
        state.OnReset += state_OnReset;
        CurrentState = state;
        CurrentState.RunManuallyModified += CurrentState_RunModified;
        UpdateSumOfBestValue(state);
    }

    private void CurrentState_RunModified(object sender, EventArgs e)
    {
        UpdateSumOfBestValue(CurrentState);
    }

    private void state_OnReset(object sender, TimerPhase e)
    {
        UpdateSumOfBestValue((LiveSplitState)sender);
    }

    private void state_OnUndoSplit(object sender, EventArgs e)
    {
        UpdateSumOfBestValue((LiveSplitState)sender);
    }

    private void state_OnSplit(object sender, EventArgs e)
    {
        UpdateSumOfBestValue((LiveSplitState)sender);
    }

    private void UpdateSumOfBestValue(LiveSplitState state)
    {
        SumOfBestValue = SumOfBest.CalculateSumOfBest(state.Run, state.Settings.SimpleSumOfBest, true, state.CurrentTimingMethod);
        PreviousCalculationMode = state.Settings.SimpleSumOfBest;
        PreviousTimingMethod = state.CurrentTimingMethod;
    }

    private bool CheckIfRunChanged(LiveSplitState state)
    {
        if (PreviousCalculationMode != state.Settings.SimpleSumOfBest)
        {
            return true;
        }

        if (PreviousTimingMethod != state.CurrentTimingMethod)
        {
            return true;
        }

        return false;
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

        InternalComponent.DisplayTwoRows = Settings.Display2Rows;

        InternalComponent.NameLabel.HasShadow
            = InternalComponent.ValueLabel.HasShadow
            = state.LayoutSettings.DropShadows;

        Formatter.Accuracy = Settings.Accuracy;

        InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

        InternalComponent.DrawVertical(g, state, width, clipRegion);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawBackground(g, state, HorizontalWidth, height);

        InternalComponent.NameLabel.HasShadow
            = InternalComponent.ValueLabel.HasShadow
            = state.LayoutSettings.DropShadows;

        Formatter.Accuracy = Settings.Accuracy;

        InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

        InternalComponent.DrawHorizontal(g, state, height, clipRegion);
    }

    public float VerticalHeight => InternalComponent.VerticalHeight;

    public float MinimumWidth => InternalComponent.MinimumWidth;

    public float HorizontalWidth => InternalComponent.HorizontalWidth;

    public float MinimumHeight => InternalComponent.MinimumHeight;

    public string ComponentName => "Sum of Best";

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
        if (CheckIfRunChanged(state))
        {
            UpdateSumOfBestValue(state);
        }

        InternalComponent.TimeValue = SumOfBestValue;

        InternalComponent.Update(invalidator, state, width, height, mode);
    }

    public void Dispose()
    {
        CurrentState.OnSplit -= state_OnSplit;
        CurrentState.OnUndoSplit -= state_OnUndoSplit;
        CurrentState.OnReset -= state_OnReset;
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
