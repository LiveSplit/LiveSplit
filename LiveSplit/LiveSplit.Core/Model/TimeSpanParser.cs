using System;
using System.Linq;
using System.Globalization;
using LiveSplit.TimeFormatters;
using System.Collections.Generic;

namespace LiveSplit.Model
{
    public static class TimeSpanParser
    {
        public static TimeSpan? ParseNullable(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return null;
            return Parse(timeString);
        }

        public static TimeSpan Parse(string timeString)
        {
            timeString = timeString.Replace(TimeFormatConstants.MINUS, "-");

            var factor = 1;
            if (timeString.StartsWith("-"))
            {
                factor = -1;
                timeString = timeString.Substring(1);
            }

            var splitTimeString = timeString.Split(new char[] {'.'}, 2);
            var secondsText = splitTimeString[0];
            var secondsTicks = ParseSecondsAsTicks(secondsText);

            var fractionTicks = 0UL;
            if (splitTimeString.Length > 1)
            {
                var fractionText = splitTimeString[1];
                fractionTicks = ParseFractionAsTicks(fractionText);
            }

            return TimeSpan.FromTicks(factor * (long)(secondsTicks + fractionTicks));
        }

        private static ulong ParseFractionAsTicks(string fractionText)
        {
            if (fractionText.Length > 7)
                fractionText = fractionText.Substring(0, 7);

            return ulong.Parse(fractionText, NumberStyles.Integer, CultureInfo.InvariantCulture) * powersOfTen[7 - fractionText.Length];
        }

        private static ulong ParseSecondsAsTicks(string secondsText)
        {
            var totalSeconds = secondsText
                .Split(':')
                .Select(x => ulong.Parse(x, NumberStyles.Integer, CultureInfo.InvariantCulture))
                .Aggregate((a, b) => 60 * a + b);

            return totalSeconds * TimeSpan.TicksPerSecond;
        }

        private static readonly ulong[] powersOfTen =
        {
            1UL,
            10UL,
            100UL,
            1000UL,
            10000UL,
            100000UL,
            1000000UL,
            10000000UL,
        };
    }
}
