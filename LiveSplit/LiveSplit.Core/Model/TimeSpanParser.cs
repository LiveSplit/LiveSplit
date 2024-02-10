using System;
using System.Linq;
using System.Globalization;
using LiveSplit.TimeFormatters;
using System.Collections.Generic;

namespace LiveSplit.Model
{
    public static class TimeSpanParser
    {
        private static readonly char[] separators = { ':' };
        private static readonly char[] dot = { '.' };

        public static TimeSpan? ParseNullable(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return null;
            return Parse(timeString);
        }

        public static TimeSpan Parse(string timeString)
        {
            timeString = timeString.Replace(TimeFormatConstants.MINUS, "-");

            long factor = 1L;
            if (timeString.StartsWith("-"))
            {
                factor = -1L;
                timeString = timeString.Substring(1);
            }

            long ticks = 0L;
            string[] sections = timeString.Split(separators, 3);
            
            switch (sections.Length)
            {
                case 3:
                    string[] daysDotHours = sections[0].Split(dot, 2);

                    if (daysDotHours.Length == 2)
                    {
                        ticks += long.Parse(daysDotHours[0], CultureInfo.InvariantCulture) * TimeSpan.TicksPerDay;
                    }

                    ticks += long.Parse(daysDotHours[daysDotHours.Length - 1], CultureInfo.InvariantCulture) * TimeSpan.TicksPerHour;
                    goto case 2;
                case 2:
                    ticks += long.Parse(sections[sections.Length - 2], CultureInfo.InvariantCulture) * TimeSpan.TicksPerMinute;
                    break;
            }

            string[] seconds = sections[sections.Length - 1].Split(dot, 3);
            ticks += long.Parse(seconds[0], CultureInfo.InvariantCulture) * TimeSpan.TicksPerSecond;

            if (seconds.Length > 1)
            {
                ticks += ParseFractionAsTicks(seconds[1]);
            }

            return TimeSpan.FromTicks(factor * ticks);
        }

        private static long ParseFractionAsTicks(string fractionText)
        {
            if (fractionText.Length > 7)
                fractionText = fractionText.Substring(0, 7);

            return long.Parse(fractionText, NumberStyles.Integer, CultureInfo.InvariantCulture) * powersOfTen[7 - fractionText.Length];
        }

        private static readonly long[] powersOfTen =
        {
            1L,
            10L,
            100L,
            1000L,
            10000L,
            100000L,
            1000000L,
            10000000L,
        };
    }
}
