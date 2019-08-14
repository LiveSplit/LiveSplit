using System;

namespace LiveSplit.TimeFormatters
{
    [Obsolete("Switch over to DigitsFormat")]
    public enum TimeFormat
    {
        TenHours,
        Hours,
        Minutes,
        Seconds
    }

    public enum DigitsFormat
    {
        /// `1`
        SingleDigitSeconds,
        /// `01`
        DoubleDigitSeconds,
        /// `0:01`
        SingleDigitMinutes,
        /// `00:01`
        DoubleDigitMinutes,
        /// `0:00:01`
        SingleDigitHours,
        /// `00:00:01`
        DoubleDigitHours,
    }

    static class FormatUtils
    {
        public static DigitsFormat ToDigitsFormat(this TimeFormat timeFormat)
        {
            if (timeFormat == TimeFormat.Seconds)
                return DigitsFormat.SingleDigitSeconds;
            else if (timeFormat == TimeFormat.Minutes)
                return DigitsFormat.DoubleDigitMinutes;
            else if (timeFormat == TimeFormat.Hours)
                return DigitsFormat.SingleDigitHours;
            else if (timeFormat == TimeFormat.TenHours)
                return DigitsFormat.DoubleDigitHours;

            return DigitsFormat.SingleDigitSeconds;
        }
    }
}
