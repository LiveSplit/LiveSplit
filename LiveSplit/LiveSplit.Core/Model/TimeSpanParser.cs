using System;
using System.Linq;
using System.Globalization;
using LiveSplit.TimeFormatters;

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

            var seconds = timeString
                .Split(':')
                .Select(x => double.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                .Aggregate((a, b) => 60 * a + b);

            return TimeSpan.FromSeconds(factor * seconds);
        }
    }
}
