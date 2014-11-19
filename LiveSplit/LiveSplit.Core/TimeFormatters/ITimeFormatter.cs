using System;

namespace LiveSplit.TimeFormatters
{
    public interface ITimeFormatter
    {
        String Format(TimeSpan? time);
    }
}
