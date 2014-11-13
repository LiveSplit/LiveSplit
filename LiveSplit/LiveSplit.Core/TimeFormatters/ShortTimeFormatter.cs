using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.TimeFormatters
{
    public class ShortTimeFormatter : ITimeFormatter
    {
        public string Format(TimeSpan? time)
        {
            //APRIL FOOLS
            /*if (DateTime.Now.Date.Month == 4 && DateTime.Now.Date.Day == 1 && time.HasValue)
            {
                time = time.Value + TimeSpan.FromSeconds(Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds));
            }*/

            if (time.HasValue)
            {
                String minusString = "";
                if (time.Value < TimeSpan.Zero)
                {
                    minusString = "-";
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
            //APRIL FOOLS
            /*if (DateTime.Now.Date.Month == 4 && DateTime.Now.Date.Day == 1 && time.HasValue)
            {
                time = time.Value + TimeSpan.FromSeconds(Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds));
            }*/

            if (time.HasValue)
            {
                String minusString = "";
                if (time.Value < TimeSpan.Zero)
                {
                    minusString = "-";
                    time = TimeSpan.Zero - time;
                }
                if (time.Value.TotalDays >= 1)
                    return minusString + (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (time.Value.TotalHours >= 1 || format == TimeFormat.Hours)
                    return minusString + time.Value.ToString(@"h\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (format == TimeFormat.Minutes)
                    return minusString + time.Value.ToString(@"mm\:ss\.ff", CultureInfo.InvariantCulture);
                else if (time.Value.Minutes >= 1)
                    return minusString + time.Value.ToString(@"m\:ss\.ff", CultureInfo.InvariantCulture);
                return minusString + time.Value.ToString(@"s\.ff", CultureInfo.InvariantCulture);
            }
            return "0.00";
        }
    }
}
