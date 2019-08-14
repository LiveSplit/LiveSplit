using System;

namespace LiveSplit.TimeFormatters
{
    public class PossibleTimeSaveFormatter : GeneralTimeFormatter
    {
        public PossibleTimeSaveFormatter() : base()
        {
            Accuracy = TimeAccuracy.Seconds;
            NullFormat = NullFormat.Dash;
        }
    }
}
