using System;
using System.Linq;
using System.Globalization;

namespace LiveSplit.Model
{
    public static class TimeSpanParser
    {
        public static TimeSpan? ParseNullable(String timeString)
        {
            if (String.IsNullOrEmpty(timeString))
                return null;
            return Parse(timeString);
        }
        public static TimeSpan Parse(String timeString)
        {
            var factor = 1;
            if (timeString.StartsWith("-"))
            {
                factor = -1;
                timeString = timeString.Substring(1);
            }

            var seconds = timeString
                .Split(':')
                .Select(x => Double.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                .Aggregate((a, b) => 60 * a + b);

            return TimeSpan.FromSeconds(factor * seconds);
        }
    }
}
