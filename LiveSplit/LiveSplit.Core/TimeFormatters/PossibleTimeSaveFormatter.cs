using System;

namespace LiveSplit.TimeFormatters
{
    public class PossibleTimeSaveFormatter : GeneralTimeFormatter
    {
        public PossibleTimeSaveFormatter()
        {
            Accuracy = TimeAccuracy.Seconds;
            NullFormat = NullFormat.Dash;
        }
    }
}
