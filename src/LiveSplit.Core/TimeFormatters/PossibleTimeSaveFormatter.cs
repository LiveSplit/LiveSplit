﻿namespace LiveSplit.TimeFormatters;

public class PossibleTimeSaveFormatter : GeneralTimeFormatter
{
    public PossibleTimeSaveFormatter()
    {
        Accuracy = TimeAccuracy.Seconds;
        DropDecimals = false;
        NullFormat = NullFormat.Dash;
    }
}
