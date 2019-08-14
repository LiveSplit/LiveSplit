using System;

namespace LiveSplit.TimeFormatters
{
    public class AutomaticPrecisionTimeFormatter : GeneralTimeFormatter
    {
        public AutomaticPrecisionTimeFormatter()
        {
            NullFormat = NullFormat.ZeroWithAccuracy;
            DigitsFormat = DigitsFormat.SingleDigitMinutes;
            Accuracy = TimeAccuracy.Hundredths;
            AutomaticPrecision = true;
            NullFormat = NullFormat.ZeroWithAccuracy;
        }

    }
}
