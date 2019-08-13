using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters
{
    public class DeltaTimeFormatter : GeneralTimeFormatter
    {

        public DeltaTimeFormatter() : base()
        {
            Accuracy = TimeAccuracy.Tenths;
            DropDecimals = true;
            ShowPlus = true;
        }
    }
}
