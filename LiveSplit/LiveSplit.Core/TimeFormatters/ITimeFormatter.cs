using System;

namespace LiveSplit.TimeFormatters
{
    public interface ITimeFormatter
    {
        string Format(TimeSpan? time);
    }
}
