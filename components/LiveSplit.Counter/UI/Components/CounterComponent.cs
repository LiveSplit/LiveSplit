using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Input;

namespace LiveSplit.UI.Components;

public class CounterComponent : IComponent
{
    public CounterComponent(LiveSplitState state)
    {
        VerticalHeight = 10;
        Settings = new CounterComponentSettings(state.Settings.HotkeyProfiles.First().Value.AllowGamepadsAsHotkeys);
        Cache = new GraphicsCache();
        CounterNameLabel = new SimpleLabel();
        Counter = new Counter();
        this.state = state;
        Settings.CounterReinitialiseRequired += Settings_CounterReinitialiseRequired;
        Settings.IncrementUpdateRequired += Settings_IncrementUpdateRequired;

        // Subscribe to input hooks.
        Settings.Hook.KeyOrButtonPressed += hook_KeyOrButtonPressed;
    }

    public ICounter Counter { get; set; }
    public CounterComponentSettings Settings { get; set; }

    public GraphicsCache Cache { get; set; }

    public float VerticalHeight { get; set; }

    public float MinimumHeight { get; set; }

    public float MinimumWidth => CounterNameLabel.X + CounterValueLabel.ActualWidth;

    public float HorizontalWidth { get; set; }

    public IDictionary<string, Action> ContextMenuControls => null;

    public float PaddingTop { get; set; }
    public float PaddingLeft => 7f;
    public float PaddingBottom { get; set; }
    public float PaddingRight => 7f;

    protected SimpleLabel CounterNameLabel = new();
    protected SimpleLabel CounterValueLabel = new();

    protected Font CounterFont { get; set; }

    private LiveSplitState state;

    private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        // Set Background colour.
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

        // Set Font.
        CounterFont = Settings.OverrideCounterFont ? Settings.CounterFont : state.LayoutSettings.TextFont;

        // Calculate Height from Font.
        float textHeight = g.MeasureString("A", CounterFont).Height;
        VerticalHeight = 1.2f * textHeight;
        MinimumHeight = MinimumHeight;

        PaddingTop = Math.Max(0, (VerticalHeight - (0.75f * textHeight)) / 2f);
        PaddingBottom = PaddingTop;

        // Assume most users won't count past four digits (will cause a layout resize in Horizontal Mode).
        float fourCharWidth = g.MeasureString("1000", CounterFont).Width;
        HorizontalWidth = CounterNameLabel.X + CounterNameLabel.ActualWidth + (fourCharWidth > CounterValueLabel.ActualWidth ? fourCharWidth : CounterValueLabel.ActualWidth) + 5;

        // Set Counter Name Label
        CounterNameLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Near : StringAlignment.Near;
        CounterNameLabel.VerticalAlignment = StringAlignment.Center;
        CounterNameLabel.X = 5;
        CounterNameLabel.Y = 0;
        CounterNameLabel.Width = width - fourCharWidth - 5;
        CounterNameLabel.Height = height;
        CounterNameLabel.Font = CounterFont;
        CounterNameLabel.Brush = new SolidBrush(Settings.OverrideTextColor ? Settings.CounterTextColor : state.LayoutSettings.TextColor);
        CounterNameLabel.HasShadow = state.LayoutSettings.DropShadows;
        CounterNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        CounterNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        CounterNameLabel.Draw(g);

        // Set Counter Value Label.
        CounterValueLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Far : StringAlignment.Far;
        CounterValueLabel.VerticalAlignment = StringAlignment.Center;
        CounterValueLabel.X = 5;
        CounterValueLabel.Y = 0;
        CounterValueLabel.Width = width - 10;
        CounterValueLabel.Height = height;
        CounterValueLabel.Font = CounterFont;
        CounterValueLabel.Brush = new SolidBrush(Settings.OverrideTextColor ? Settings.CounterValueColor : state.LayoutSettings.TextColor);
        CounterValueLabel.HasShadow = state.LayoutSettings.DropShadows;
        CounterValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        CounterValueLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        CounterValueLabel.Draw(g);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
    }

    public string ComponentName => "Counter";

    public Control GetSettingsControl(LayoutMode mode)
    {
        return Settings;
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        Settings.SetSettings(settings);

        // Initialise Counter from settings.
        Counter = new Counter(Settings.InitialValue, Settings.Increment);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        try
        {
            Settings.Hook?.Poll();
        }
        catch { }

        this.state = state;

        CounterNameLabel.Text = Settings.CounterText;
        CounterValueLabel.Text = Counter.Count.ToString();

        Cache.Restart();
        Cache["CounterNameLabel"] = CounterNameLabel.Text;
        Cache["CounterValueLabel"] = CounterValueLabel.Text;

        if (invalidator != null && Cache.HasChanged)
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    public void Dispose()
    {
        Settings.Hook.KeyOrButtonPressed -= hook_KeyOrButtonPressed;
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }

    /// <summary>
    /// Handles the CounterReinitialiseRequired event of the Settings control.
    /// </summary>
    private void Settings_CounterReinitialiseRequired(object sender, EventArgs e)
    {
        Counter = new Counter(Settings.InitialValue, Settings.Increment);
    }

    private void Settings_IncrementUpdateRequired(object sender, EventArgs e)
    {
        Counter.SetIncrement(Settings.Increment);
    }

    // Basic support for keyboard/button input.
    private void hook_KeyOrButtonPressed(object sender, KeyOrButton e)
    {
        if ((Form.ActiveForm == state.Form && !Settings.GlobalHotkeysEnabled)
            || Settings.GlobalHotkeysEnabled)
        {
            if (e == Settings.IncrementKey)
            {
                Counter.Increment();
            }

            if (e == Settings.DecrementKey)
            {
                Counter.Decrement();
            }

            if (e == Settings.ResetKey)
            {
                Counter.Reset();
            }
        }
    }
}
