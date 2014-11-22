using System;
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
            double num = 0.0;
            var factor = 1;
            if (timeString.StartsWith("-"))
            {
                factor = -1;
                timeString = timeString.Substring(1);
            }

            string[] array = timeString.Split(':');
            foreach (string s in array)
            {
                double num2;
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num2))
                {
                    num = num * 60.0 + num2;
                }
                else
                {
                    throw new Exception();
                }
            }

            if (factor * num > 864000)
                throw new Exception();

            return new TimeSpan((long)(factor * num * 10000000));
        }
    }
}
