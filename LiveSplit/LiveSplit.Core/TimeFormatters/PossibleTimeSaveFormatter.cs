using System;

namespace LiveSplit.TimeFormatters
{
    public class PossibleTimeSaveFormatter : GeneralTimeFormatter
    {
        public PossibleTimeSaveFormatter()
        {
            Accuracy = TimeAccuracy.Seconds;
            DropDecimals = false;
            DropDecimalsPossibleTimeSave = false;
            NullFormat = NullFormat.Dash;
        }
    }
}
