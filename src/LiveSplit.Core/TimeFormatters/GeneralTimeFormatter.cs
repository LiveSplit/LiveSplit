using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters;

public class GeneralTimeFormatter : ITimeFormatter
{
    private static readonly CultureInfo ic = CultureInfo.InvariantCulture;

    public TimeAccuracy Accuracy { get; set; }

    public DigitsFormat DigitsFormat { get; set; }

    /// <summary>
    /// How to display null times
    /// </summary>
    public NullFormat NullFormat { get; set; }

    /// <summary>
    /// If true, for example show "1d 23:59:10" instead of "47:59:10". For durations of 24 hours or more,
    /// </summary>
    public bool ShowDays { get; set; }

    /// <summary>
    /// If true, include a "+" for positive times (excluding zero)
    /// </summary>
    public bool ShowPlus { get; set; }

    /// <summary>
    /// If true, don't display decimals if absolute time is 1 minute or more
    /// </summary>
    public bool DropDecimals { get; set; }

    /// <summary>
    /// If true, don't display trailing zero decimal places
    /// </summary>
    public bool AutomaticPrecision { get; set; } = false;

    public GeneralTimeFormatter()
    {
        DigitsFormat = DigitsFormat.SingleDigitSeconds;
        NullFormat = NullFormat.Dash;
    }

    public string Format(TimeSpan? timeNullable)
    {
        bool isNull = !timeNullable.HasValue;
        if (isNull)
        {
            if (NullFormat == NullFormat.Dash)
            {
                return TimeFormatConstants.DASH;
            }
            else if (NullFormat == NullFormat.ZeroWithAccuracy)
            {
                return ZeroWithAccuracy();
            }
            else if (NullFormat == NullFormat.ZeroDotZeroZero)
            {
                return "0.00";
            }
            else if (NullFormat is NullFormat.ZeroValue or NullFormat.Dashes)
            {
                timeNullable = TimeSpan.Zero;
            }
        }

        TimeSpan time = timeNullable.Value;

        string minusString;
        if (time == TimeSpan.Zero)
        {
            minusString = "";
        }
        else if (time < TimeSpan.Zero)
        {
            minusString = TimeFormatConstants.MINUS;
            time = -time;
        }
        else
        {
            minusString = ShowPlus ? "+" : "";
        }

        string decimalFormat = "";
        if (AutomaticPrecision)
        {
            double totalSeconds = time.TotalSeconds;
            if (Accuracy == TimeAccuracy.Seconds || totalSeconds % 1 == 0)
            {
                decimalFormat = "";
            }
            else if (Accuracy == TimeAccuracy.Tenths || 10 * totalSeconds % 1 == 0)
            {
                decimalFormat = @"\.f";
            }
            else if (Accuracy == TimeAccuracy.Hundredths || 100 * totalSeconds % 1 == 0)
            {
                decimalFormat = @"\.ff";
            }
            else
            {
                decimalFormat = @"\.fff";
            }
        }
        else
        {
            if (DropDecimals && time.TotalMinutes >= 1)
            {
                decimalFormat = "";
            }
            else if (Accuracy == TimeAccuracy.Seconds)
            {
                decimalFormat = "";
            }
            else if (Accuracy == TimeAccuracy.Tenths)
            {
                decimalFormat = @"\.f";
            }
            else if (Accuracy == TimeAccuracy.Hundredths)
            {
                decimalFormat = @"\.ff";
            }
            else if (Accuracy == TimeAccuracy.Milliseconds)
            {
                decimalFormat = @"\.fff";
            }
        }

        string formatted;
        if (time.TotalDays >= 1 && !ShowDays)
        {
            // Days rolled into the hour count, e.g. "47:59:10".
            formatted = minusString + (int)time.TotalHours + time.ToString(@"\:mm\:ss" + decimalFormat, ic);
        }
        else
        {
            // Arms are ordered by precedence: a larger actual magnitude (days/hours/minutes >= 1) forces a
            // bigger unit than DigitsFormat would otherwise pick, so those guards come before the enum arms.
            string format = DigitsFormat switch
            {
                DigitsFormat.DoubleDigitHours when time.TotalDays >= 1 => @"d\d\ hh\:mm\:ss",
                _ when time.TotalDays >= 1 => @"d\d\ h\:mm\:ss",
                DigitsFormat.DoubleDigitHours => @"hh\:mm\:ss",
                _ when time.TotalHours >= 1 => @"h\:mm\:ss",
                DigitsFormat.SingleDigitHours => @"h\:mm\:ss",
                DigitsFormat.DoubleDigitMinutes => @"mm\:ss",
                _ when time.TotalMinutes >= 1 => @"m\:ss",
                DigitsFormat.SingleDigitMinutes => @"m\:ss",
                DigitsFormat.DoubleDigitSeconds => @"ss",
                _ => @"%s",
            };

            formatted = minusString + time.ToString(format + decimalFormat, ic);
        }

        if (isNull && NullFormat == NullFormat.Dashes)
        {
            formatted = formatted.Replace('0', '-');
        }

        return formatted;
    }

    private string ZeroWithAccuracy()
    {
        if (AutomaticPrecision || Accuracy == TimeAccuracy.Seconds)
        {
            return "0";
        }
        else if (Accuracy == TimeAccuracy.Tenths)
        {
            return "0.0";
        }
        else if (Accuracy == TimeAccuracy.Milliseconds)
        {
            return "0.000";
        }
        else
        {
            return "0.00";
        }
    }
}
