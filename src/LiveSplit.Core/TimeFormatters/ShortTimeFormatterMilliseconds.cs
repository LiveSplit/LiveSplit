namespace LiveSplit.TimeFormatters;

public class ShortTimeFormatterMilliseconds : GeneralTimeFormatter
{
    public ShortTimeFormatterMilliseconds(DigitsFormat format = DigitsFormat.SingleDigitSeconds)
    {
        Accuracy = TimeAccuracy.Milliseconds;
        NullFormat = NullFormat.ZeroWithAccuracy;
        DigitsFormat = format;
    }
}
