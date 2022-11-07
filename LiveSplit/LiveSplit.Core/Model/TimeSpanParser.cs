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

            var fractionTicks = 0L;
            if (splitTimeString.Length > 1)
            {
                var fractionText = splitTimeString[1];
                fractionTicks = ParseFractionAsTicks(fractionText);
            }

            return TimeSpan.FromTicks((long)(factor * (secondsTicks + fractionTicks)));
        }

        private static long ParseFractionAsTicks(string fractionText)
        {
            if (fractionText.Length > 7)
                fractionText = fractionText.Substring(0, 7);

            return long.Parse(fractionText) * powersOfTen[7 - fractionText.Length];
        }

        private static long ParseSecondsAsTicks(string secondsText)
        {
            var totalSeconds = secondsText
                .Split(':')
                .Select(x => long.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                .Aggregate((a, b) => 60 * a + b);

            return totalSeconds * TimeSpan.TicksPerSecond;
        }

        private static readonly Dictionary<int, long> powersOfTen = new Dictionary<int, long>()
        {
            { 0, 1L },
            { 1, 10L },
            { 2, 100L },
            { 3, 1000L },
            { 4, 10000L },
            { 5, 100000L },
            { 6, 1000000L },
            { 7, 10000000L },
        };
    }
}
