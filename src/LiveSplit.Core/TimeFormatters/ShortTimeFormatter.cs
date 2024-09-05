using System;

namespace LiveSplit.TimeFormatters;

public class ShortTimeFormatter : ITimeFormatter
{
    public ShortTimeFormatter() { }

    [Obsolete("Switch over to DigitsFormat")]
    public string Format(TimeSpan? time, TimeFormat format)
    {
        var formatRequest = new GeneralTimeFormatter
        {
            Accuracy = TimeAccuracy.Hundredths,
            NullFormat = NullFormat.ZeroWithAccuracy,
            DigitsFormat = format.ToDigitsFormat(),
        };

        return formatRequest.Format(time);
    }

    public string Format(TimeSpan? time, DigitsFormat format)
    {
        var formatRequest = new GeneralTimeFormatter
        {
            Accuracy = TimeAccuracy.Hundredths,
            NullFormat = NullFormat.ZeroWithAccuracy,
            DigitsFormat = format,
        };

        return formatRequest.Format(time);
    }

    public string Format(TimeSpan? timeSpan)
    {
        return Format(timeSpan, DigitsFormat.SingleDigitSeconds);
    }
}
