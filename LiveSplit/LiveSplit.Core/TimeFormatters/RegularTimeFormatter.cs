using System;

namespace LiveSplit.TimeFormatters
{
    public class RegularTimeFormatter : GeneralTimeFormatter
    {
        public RegularTimeFormatter(TimeAccuracy accuracy = TimeAccuracy.Seconds)
        {
            Accuracy = accuracy;
            NullFormat = NullFormat.ZeroWithAccuracy;
            DigitsFormat = DigitsFormat.SingleDigitMinutes;
        }
    }
}
