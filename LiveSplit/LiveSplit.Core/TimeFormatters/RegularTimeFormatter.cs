using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters
{
    public class RegularTimeFormatter : ITimeFormatter
    {
        public TimeAccuracy Accuracy { get; set; }

        public RegularTimeFormatter(TimeAccuracy accuracy = TimeAccuracy.Seconds)
        {
            Accuracy = accuracy;
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
                if (Accuracy == TimeAccuracy.Hundredths)
                {
                    if (time.Value.TotalDays >= 1)
                        return (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                    else if (time.Value.TotalHours >= 1)
                        return time.Value.ToString(@"h\:mm\:ss\.ff", CultureInfo.InvariantCulture);
                    return time.Value.ToString(@"m\:ss\.ff", CultureInfo.InvariantCulture);
                }
                else if (Accuracy == TimeAccuracy.Seconds)
                {
                    if (time.Value.TotalDays >= 1)
                        return (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss", CultureInfo.InvariantCulture);
                    else if (time.Value.TotalHours >= 1)
                        return time.Value.ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture);
                    return time.Value.ToString(@"m\:ss", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (time.Value.TotalDays >= 1)
                        return (int)(time.Value.TotalHours) + time.Value.ToString(@"\:mm\:ss\.f", CultureInfo.InvariantCulture);
                    else if (time.Value.TotalHours >= 1)
                        return time.Value.ToString(@"h\:mm\:ss\.f", CultureInfo.InvariantCulture);
                    return time.Value.ToString(@"m\:ss\.f", CultureInfo.InvariantCulture);
                }
            }
            if (Accuracy == TimeAccuracy.Seconds)
                return "0";
            if (Accuracy == TimeAccuracy.Tenths)
                return "0.0";
            return "0.00";
        }
    }
}
