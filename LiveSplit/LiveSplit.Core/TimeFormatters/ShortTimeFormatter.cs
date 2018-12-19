﻿using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters
{
    public class ShortTimeFormatter : ITimeFormatter
    {
        public string Format(TimeSpan? time)
        {
            if (time.HasValue)
            {
                string minusString = "";
                if (time.Value < TimeSpan.Zero)
                {
                    minusString = TimeFormatConstants.MINUS;
                    time = TimeSpan.Zero - time;
                }
                if (time.Value.TotalDays >= 1)
                    return minusString + (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (time.Value.TotalHours >= 1)
                    return minusString+time.Value.ToString(@"h\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (time.Value.Minutes >= 1)
                    return minusString+time.Value.ToString(@"m\:ss\.ff", CultureInfo.InvariantCulture);
                return minusString+time.Value.ToString(@"s\.ff", CultureInfo.InvariantCulture);
            }
            return "0.00";
        }

        public string Format(TimeSpan? time, TimeFormat format)
        {
            if (time.HasValue)
            {
                string minusString = "";
                if (time.Value < TimeSpan.Zero)
                {
                    minusString = TimeFormatConstants.MINUS;
                    time = TimeSpan.Zero - time;
                }
                if (time.Value.TotalDays >= 1)
                {
                    return minusString + (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                }
                else if (format == TimeFormat.TenHours)
                {
                    return minusString + time.Value.ToString(@"hh\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                }
                else if (time.Value.TotalHours >= 1 || format == TimeFormat.Hours)
                {
                    return minusString + time.Value.ToString(@"h\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                }
                else if (format == TimeFormat.Minutes)
                {
                    return minusString + time.Value.ToString(@"mm\:ss\.ff", CultureInfo.InvariantCulture);
                }
                else if (time.Value.Minutes >= 1)
                {
                    return minusString + time.Value.ToString(@"m\:ss\.ff", CultureInfo.InvariantCulture);
                }
                return minusString + time.Value.ToString(@"s\.ff", CultureInfo.InvariantCulture);
            }
            return "0.00";
        }
    }
}
