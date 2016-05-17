using System;

namespace LiveSplit.TimeFormatters
{
    public class PossibleTimeSaveFormatter : ITimeFormatter
    {
        public TimeAccuracy Accuracy { get; set; }

        public string Format(TimeSpan? time)
        {
            var formatter = new ShortTimeFormatter();
            if (time == null)
                return "−";
            else
            {
                var timeString = formatter.Format(time);
                if (Accuracy == TimeAccuracy.Hundredths)
                    return timeString;
                else if (Accuracy == TimeAccuracy.Tenths)
                    return timeString.Substring(0, timeString.Length - 1);
                else
                    return timeString.Substring(0, timeString.Length - 3);

            }
                
        }
    }
}
