using System;
using System.Drawing;

namespace LiveSplit.Options;

public class FontOverrides : ICloneable, IDisposable
{
    public bool OverrideTimerFont { get; set; }

    public Font TimerFont { get; set; }

    public bool OverrideTimesFont { get; set; }

    public Font TimesFont { get; set; }

    public bool OverrideTextFont { get; set; }

    public Font TextFont { get; set; }

    public bool HasOverrides => OverrideTimerFont || OverrideTimesFont || OverrideTextFont;

    public void ApplyTo(LayoutSettings settings, out Font origTimer, out Font origTimes, out Font origText)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        origTimer = settings.TimerFont;
        origTimes = settings.TimesFont;
        origText = settings.TextFont;

        if (OverrideTimerFont && TimerFont != null)
        {
            settings.TimerFont = TimerFont;
        }

        if (OverrideTimesFont && TimesFont != null)
        {
            settings.TimesFont = TimesFont;
        }

        if (OverrideTextFont && TextFont != null)
        {
            settings.TextFont = TextFont;
        }
    }

    public static void Restore(LayoutSettings settings, Font origTimer, Font origTimes, Font origText)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.TimerFont = origTimer;
        settings.TimesFont = origTimes;
        settings.TextFont = origText;
    }

    public object Clone()
    {
        return new FontOverrides()
        {
            OverrideTimerFont = OverrideTimerFont,
            TimerFont = TimerFont?.Clone() as Font,
            OverrideTimesFont = OverrideTimesFont,
            TimesFont = TimesFont?.Clone() as Font,
            OverrideTextFont = OverrideTextFont,
            TextFont = TextFont?.Clone() as Font
        };
    }

    public void Dispose()
    {
        TimerFont?.Dispose();
        TimesFont?.Dispose();
        TextFont?.Dispose();
    }
}
