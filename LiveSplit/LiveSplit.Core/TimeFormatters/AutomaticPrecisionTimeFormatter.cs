using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.TimeFormatters
{
    public class AutomaticPrecisionTimeFormatter : ITimeFormatter
    {
        RegularTimeFormatter InternalFormatter;

        public AutomaticPrecisionTimeFormatter()
        {
            InternalFormatter = new RegularTimeFormatter();
        }

        public string Format(TimeSpan? time)
        {
            if (time.HasValue)
            {
                var totalSeconds = time.Value.TotalSeconds;
                if (totalSeconds % 1 == 0)
                    InternalFormatter.Accuracy = TimeAccuracy.Seconds;
                else if ((10 * totalSeconds) % 1 == 0)
                    InternalFormatter.Accuracy = TimeAccuracy.Tenths;
                else
                    InternalFormatter.Accuracy = TimeAccuracy.Hundredths;
            }

            return InternalFormatter.Format(time);
        }
    }
}
