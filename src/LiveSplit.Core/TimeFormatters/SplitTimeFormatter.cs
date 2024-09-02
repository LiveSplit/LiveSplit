namespace LiveSplit.TimeFormatters;

public class SplitTimeFormatter : GeneralTimeFormatter
{
    public SplitTimeFormatter(TimeAccuracy accuracy = TimeAccuracy.Seconds)
    {
        Accuracy = accuracy;
        NullFormat = NullFormat.Dash;
        DigitsFormat = DigitsFormat.SingleDigitMinutes;
    }
}
