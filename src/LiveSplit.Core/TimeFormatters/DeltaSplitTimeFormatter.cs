namespace LiveSplit.TimeFormatters;

public class DeltaSplitTimeFormatter : GeneralTimeFormatter
{
    public DeltaSplitTimeFormatter(TimeAccuracy accuracy, bool dropDecimals)
    {
        Accuracy = accuracy;
        DropDecimals = dropDecimals;
        NullFormat = NullFormat.Dash;
        ShowPlus = true;
    }
}
