using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.TimeFormatters
{
    public class DeltaTimeFormatter : ITimeFormatter
    {
        public TimeAccuracy Accuracy { get; set; }
        public bool DropDecimals { get; set; }

        public DeltaTimeFormatter()
        {
            Accuracy = TimeAccuracy.Tenths;
            DropDecimals = true;
        }

        public string Format(TimeSpan? time)
        {
            //APRIL FOOLS
            /*if (DateTime.Now.Date.Month == 4 && DateTime.Now.Date.Day == 1 && time.HasValue)
            {
                time = time.Value + TimeSpan.FromSeconds(Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds));
            }*/

            if (time.HasValue)
            {
                String minusString = "+";
                var totalString = "";
                if (time.Value < TimeSpan.Zero)
                {
                    minusString = "-";
                    time = TimeSpan.Zero-time;
                }
                if (time.Value.TotalDays >= 1)
                    totalString = minusString + (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (time.Value.TotalHours >= 1)
                    totalString = minusString+time.Value.ToString(@"h\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (time.Value.TotalMinutes >= 1)
                    totalString = minusString+time.Value.ToString(@"m\:ss\.ff", CultureInfo.InvariantCulture);
                else
                    totalString = minusString+time.Value.ToString(@"s\.ff", CultureInfo.InvariantCulture);
                if ((DropDecimals && time.Value.TotalMinutes >= 1) || Accuracy == TimeAccuracy.Seconds)
                    return totalString.Substring(0, totalString.Length - 3);
                else if (Accuracy == TimeAccuracy.Tenths)
                    return totalString.Substring(0, totalString.Length - 1);
                return totalString;
            }

            return "-";
        }
    }
}
