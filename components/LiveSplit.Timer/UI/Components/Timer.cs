using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public class Timer : IComponent
{
    public SimpleLabel BigTextLabel { get; set; }
    public SimpleLabel SmallTextLabel { get; set; }
    protected SimpleLabel BigMeasureLabel { get; set; }
    protected GeneralTimeFormatter Formatter { get; set; }

    protected Font TimerDecimalPlacesFont { get; set; }
    protected Font TimerFont { get; set; }
    protected float PreviousDecimalsSize { get; set; }

    public Color TimerColor = Color.Transparent;

    public GraphicsCache Cache { get; set; }

    public TimerSettings Settings { get; set; }
    public float ActualWidth { get; set; }

    public string ComponentName => "Timer";

    public float VerticalHeight => Settings.TimerHeight;

    public float MinimumWidth => 20;

    public float HorizontalWidth => Settings.TimerWidth;

    public float MinimumHeight => 20;

    public float PaddingTop => 0f;
    public float PaddingLeft => 7f;
    public float PaddingBottom => 0f;
    public float PaddingRight => 7f;

    public IDictionary<string, Action> ContextMenuControls => null;

    public Timer()
    {
        BigTextLabel = new SimpleLabel()
        {
            HorizontalAlignment = StringAlignment.Far,
            VerticalAlignment = StringAlignment.Near,
            Width = 493,
            Text = "0",
        };

        SmallTextLabel = new SimpleLabel()
        {
            HorizontalAlignment = StringAlignment.Near,
            VerticalAlignment = StringAlignment.Near,
            Width = 257,
            Text = "0",
        };

        BigMeasureLabel = new SimpleLabel()
        {
            Text = "88:88:88",
            IsMonospaced = true
        };

        Formatter = new GeneralTimeFormatter
        {
            Accuracy = TimeAccuracy.Hundredths,
            NullFormat = NullFormat.ZeroWithAccuracy,
            DigitsFormat = DigitsFormat.SingleDigitSeconds
        };
        Settings = new TimerSettings();
        UpdateTimeFormat();
        Cache = new GraphicsCache();
        TimerColor = Color.Transparent;
    }

    public static void DrawBackground(Graphics g, Color timerColor, Color settingsColor1, Color settingsColor2,
        float width, float height, DeltasGradientType gradientType)
    {
        Color background1 = settingsColor1;
        Color background2 = settingsColor2;
        if (gradientType is DeltasGradientType.PlainWithDeltaColor
            or DeltasGradientType.HorizontalWithDeltaColor
            or DeltasGradientType.VerticalWithDeltaColor)
        {
            timerColor.ToHSV(out double h, out double s, out double v);
            Color newColor = ColorExtensions.FromHSV(h, s * 0.5, v * 0.25);

            if (gradientType == DeltasGradientType.PlainWithDeltaColor)
            {
                background1 = Color.FromArgb(timerColor.A * 7 / 12, newColor);
            }
            else
            {
                background1 = Color.FromArgb(timerColor.A / 6, newColor);
                background2 = Color.FromArgb(timerColor.A, newColor);
            }
        }

        if (background1.A > 0
        || (gradientType != DeltasGradientType.Plain
        && background2.A > 0))
        {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        gradientType is DeltasGradientType.Horizontal
                        or DeltasGradientType.HorizontalWithDeltaColor
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        background1,
                        gradientType is DeltasGradientType.Plain
                        or DeltasGradientType.PlainWithDeltaColor
                        ? background1
                        : background2);
            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }
    }

    private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height)
    {
        DrawBackground(g, TimerColor, Settings.BackgroundColor, Settings.BackgroundColor2, width, height, Settings.BackgroundGradient);

        if (state.LayoutSettings.TimerFont != TimerFont || Settings.DecimalsSize != PreviousDecimalsSize)
        {
            TimerFont = state.LayoutSettings.TimerFont;
            TimerDecimalPlacesFont = new Font(TimerFont.FontFamily.Name, TimerFont.Size / 50f * Settings.DecimalsSize, TimerFont.Style, GraphicsUnit.Pixel);
            PreviousDecimalsSize = Settings.DecimalsSize;
        }

        BigTextLabel.Font = BigMeasureLabel.Font = TimerFont;
        SmallTextLabel.Font = TimerDecimalPlacesFont;

        BigMeasureLabel.SetActualWidth(g);
        SmallTextLabel.SetActualWidth(g);

        Matrix oldMatrix = g.Transform;
        float unscaledWidth = Math.Max(10, BigMeasureLabel.ActualWidth + SmallTextLabel.ActualWidth + 11);
        float unscaledHeight = 45f;
        float widthFactor = (width - 14) / (unscaledWidth - 14);
        float heightFactor = height / unscaledHeight;
        float adjustValue = !Settings.CenterTimer ? 7f : 0f;
        float scale = Math.Min(widthFactor, heightFactor);
        g.TranslateTransform(width - adjustValue, height / 2);
        g.ScaleTransform(scale, scale);
        g.TranslateTransform(-unscaledWidth + adjustValue, -0.5f * unscaledHeight);
        if (Settings.CenterTimer)
        {
            g.TranslateTransform(-(width - (unscaledWidth * scale)) / 2f / scale, 0);
        }

        DrawUnscaled(g, state, unscaledWidth, unscaledHeight);
        ActualWidth = scale * (SmallTextLabel.ActualWidth + BigTextLabel.ActualWidth);
        g.Transform = oldMatrix;
    }

    public void DrawUnscaled(Graphics g, LiveSplitState state, float width, float height)
    {
        BigTextLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        BigTextLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        BigTextLabel.HasShadow = state.LayoutSettings.DropShadows;
        SmallTextLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        SmallTextLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        SmallTextLabel.HasShadow = state.LayoutSettings.DropShadows;
        Font smallFont = TimerDecimalPlacesFont;
        Font bigFont = TimerFont;
        float sizeMultiplier = bigFont.Size / bigFont.FontFamily.GetEmHeight(bigFont.Style);
        float smallSizeMultiplier = smallFont.Size / bigFont.FontFamily.GetEmHeight(bigFont.Style);
        float ascent = sizeMultiplier * bigFont.FontFamily.GetCellAscent(bigFont.Style);
        float descent = sizeMultiplier * bigFont.FontFamily.GetCellDescent(bigFont.Style);
        float smallAscent = smallSizeMultiplier * smallFont.FontFamily.GetCellAscent(smallFont.Style);
        float shift = (height - ascent - descent) / 2f;

        BigTextLabel.X = width - 499 - SmallTextLabel.ActualWidth;
        SmallTextLabel.X = width - SmallTextLabel.ActualWidth - 6;
        BigTextLabel.Y = shift;
        SmallTextLabel.Y = shift + ascent - smallAscent;
        BigTextLabel.Height = 150f;
        SmallTextLabel.Height = 150f;

        BigTextLabel.IsMonospaced = true;
        SmallTextLabel.IsMonospaced = true;

        if (Settings.ShowGradient && BigTextLabel.Brush is SolidBrush)
        {
            Color originalColor = (BigTextLabel.Brush as SolidBrush).Color;
            originalColor.ToHSV(out double h, out double s, out double v);

            Color bottomColor = ColorExtensions.FromHSV(h, s, 0.8 * v);
            Color topColor = ColorExtensions.FromHSV(h, 0.5 * s, Math.Min(1, (1.5 * v) + 0.1));

            var bigTimerGradiantBrush = new LinearGradientBrush(
                new PointF(BigTextLabel.X, BigTextLabel.Y),
                new PointF(BigTextLabel.X, BigTextLabel.Y + ascent + descent),
                topColor,
                bottomColor);
            var smallTimerGradiantBrush = new LinearGradientBrush(
                new PointF(SmallTextLabel.X, SmallTextLabel.Y),
                new PointF(SmallTextLabel.X, SmallTextLabel.Y + ascent + descent + smallFont.Size - bigFont.Size),
                topColor,
                bottomColor);

            BigTextLabel.Brush = bigTimerGradiantBrush;
            SmallTextLabel.Brush = smallTimerGradiantBrush;
        }

        BigTextLabel.Draw(g);
        SmallTextLabel.Draw(g);
    }

    protected void UpdateTimeFormat()
    {
        if (Settings.DigitsFormat == "1")
        {
            Formatter.DigitsFormat = DigitsFormat.SingleDigitSeconds;
        }
        else if (Settings.DigitsFormat == "00:01")
        {
            Formatter.DigitsFormat = DigitsFormat.DoubleDigitMinutes;
        }
        else if (Settings.DigitsFormat == "0:00:01")
        {
            Formatter.DigitsFormat = DigitsFormat.SingleDigitHours;
        }
        else
        {
            Formatter.DigitsFormat = DigitsFormat.DoubleDigitHours;
        }

        if (Settings.Accuracy == ".234")
        {
            Formatter.Accuracy = TimeAccuracy.Milliseconds;
        }
        else if (Settings.Accuracy == ".23")
        {
            Formatter.Accuracy = TimeAccuracy.Hundredths;
        }
        else if (Settings.Accuracy == ".2")
        {
            Formatter.Accuracy = TimeAccuracy.Tenths;
        }
        else
        {
            Formatter.Accuracy = TimeAccuracy.Seconds;
        }
    }

    public virtual TimeSpan? GetTime(LiveSplitState state, TimingMethod method)
    {
        if (state.CurrentPhase == TimerPhase.NotRunning)
        {
            return state.Run.Offset;
        }
        else
        {
            return state.CurrentTime[method];
        }
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height);
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

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        Cache.Restart();

        TimingMethod timingMethod = state.CurrentTimingMethod;
        if (Settings.TimingMethod == "Real Time")
        {
            timingMethod = TimingMethod.RealTime;
        }
        else if (Settings.TimingMethod == "Game Time")
        {
            timingMethod = TimingMethod.GameTime;
        }

        UpdateTimeFormat();

        TimeSpan? timeValue = GetTime(state, timingMethod);

        if (timeValue == null && timingMethod == TimingMethod.GameTime)
        {
            timeValue = GetTime(state, TimingMethod.RealTime);
        }

        if (timeValue != null)
        {
            string timeString = Formatter.Format(timeValue);
            if (Formatter.Accuracy != TimeAccuracy.Seconds)
            {
                int dotIndex = timeString.IndexOf(".");
                BigTextLabel.Text = timeString[..dotIndex];
                SmallTextLabel.Text = timeString[dotIndex..];
            }
            else
            {
                BigTextLabel.Text = timeString;
                SmallTextLabel.Text = "";
            }
        }
        else
        {
            SmallTextLabel.Text = TimeFormatConstants.DASH;
            BigTextLabel.Text = "";
        }

        if (state.CurrentPhase == TimerPhase.NotRunning || state.CurrentTime[timingMethod] < TimeSpan.Zero)
        {
            TimerColor = state.LayoutSettings.NotRunningColor;
        }
        else if (state.CurrentPhase == TimerPhase.Paused)
        {
            TimerColor = state.LayoutSettings.PausedColor;
        }
        else if (state.CurrentPhase == TimerPhase.Ended)
        {
            if (state.Run.Last().Comparisons[state.CurrentComparison][timingMethod] == null || state.CurrentTime[timingMethod] < state.Run.Last().Comparisons[state.CurrentComparison][timingMethod])
            {
                TimerColor = state.LayoutSettings.PersonalBestColor;
            }
            else
            {
                TimerColor = state.LayoutSettings.BehindLosingTimeColor;
            }
        }
        else if (state.CurrentPhase == TimerPhase.Running)
        {
            if (state.CurrentSplit.Comparisons[state.CurrentComparison][timingMethod] != null)
            {
                TimerColor = LiveSplitStateHelper.GetSplitColor(state, state.CurrentTime[timingMethod] - state.CurrentSplit.Comparisons[state.CurrentComparison][timingMethod],
                    state.CurrentSplitIndex, true, false, state.CurrentComparison, timingMethod)
                    ?? state.LayoutSettings.AheadGainingTimeColor;
            }
            else
            {
                TimerColor = state.LayoutSettings.AheadGainingTimeColor;
            }
        }

        if (Settings.OverrideSplitColors)
        {
            BigTextLabel.ForeColor = Settings.TimerColor;
            SmallTextLabel.ForeColor = Settings.TimerColor;
        }
        else
        {
            BigTextLabel.ForeColor = TimerColor;
            SmallTextLabel.ForeColor = TimerColor;
        }

        Cache["TimerText"] = BigTextLabel.Text + SmallTextLabel.Text;
        if (BigTextLabel.Brush != null && invalidator != null)
        {
            Cache["TimerColor"] = BigTextLabel.ForeColor.ToArgb();
        }

        if (invalidator != null && Cache.HasChanged)
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
