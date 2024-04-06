using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters
{
    public class ShortTimeFormatter : GeneralTimeFormatter
    {
        public ShortTimeFormatter()
        {
            Accuracy = TimeAccuracy.Hundredths;
            NullFormat = NullFormat.ZeroWithAccuracy;
        }

        [Obsolete("Switch over to DigitsFormat")]
        public string Format(TimeSpan? time, TimeFormat format)
        {
            var formatRequest = new GeneralTimeFormatter {
                Accuracy = TimeAccuracy.Hundredths,
                NullFormat = NullFormat.ZeroWithAccuracy,
                DigitsFormat = format.ToDigitsFormat(),
            };

            return formatRequest.Format(time);
        }

        public string Format(TimeSpan? time, DigitsFormat format)
        {
            var formatRequest = new GeneralTimeFormatter
            {
                Accuracy = TimeAccuracy.Hundredths,
                NullFormat = NullFormat.ZeroWithAccuracy,
                DigitsFormat = format,
            };

            return formatRequest.Format(time);
        }
    }
}
