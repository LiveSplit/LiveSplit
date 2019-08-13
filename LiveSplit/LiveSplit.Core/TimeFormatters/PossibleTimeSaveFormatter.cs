using System;

namespace LiveSplit.TimeFormatters
{
    public class PossibleTimeSaveFormatter : GeneralTimeFormatter
    {
        public PossibleTimeSaveFormatter() : base()
        {
            Accuracy = TimeAccuracy.Hundredths;
            NullFormat = NullFormat.Dash;
        }
    }
}
