using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveSplit.TimeFormatters
{
    [Obsolete("Switch over to DigitsFormat")]
    public enum TimeFormat
    {
        TenHours,
        Hours,
        Minutes,
        Seconds,
        Days,
        Years
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
        /// `1d 00:00:01`
        DayDoubleDigitHours,
        /// `100y100d 00`
        YearDayHours,
    }

    public static class FormatUtils
    {
        public static DigitsFormat ToDigitsFormat(this TimeFormat timeFormat)
        {
            switch (timeFormat)
            {
                case TimeFormat.Seconds:
                    return DigitsFormat.SingleDigitSeconds;
                case TimeFormat.Minutes:
                    return DigitsFormat.DoubleDigitMinutes;
                case TimeFormat.Hours:
                    return DigitsFormat.SingleDigitHours;
                case TimeFormat.TenHours:
                    return DigitsFormat.DoubleDigitHours;
                case TimeFormat.Days:
                    return DigitsFormat.DayDoubleDigitHours;
                case TimeFormat.Years:
                    return DigitsFormat.YearDayHours;
                default:
                    return DigitsFormat.SingleDigitSeconds;
            }            
        }
        /// <summary> 
        /// Dictionary that contains the format strings used in the settings
        /// </summary>
        public static readonly Dictionary<DigitsFormat, string> StringsDict = new Dictionary<DigitsFormat, string> {
            { DigitsFormat.SingleDigitSeconds , "1" },      { DigitsFormat.DoubleDigitSeconds ,"01" },
            { DigitsFormat.SingleDigitMinutes ,"0:01" },    { DigitsFormat.DoubleDigitMinutes , "00:01" },
            { DigitsFormat.SingleDigitHours ,"0:00:01"},     { DigitsFormat.DoubleDigitHours ,"00:00:01"},
            { DigitsFormat.DayDoubleDigitHours ,  "1d 00:00:01" },
            { DigitsFormat.YearDayHours ,"100y100d 00" },
        };

    }
}
