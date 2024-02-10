using System;

namespace LiveSplit.TimeFormatters
{
    public interface ITimeFormatter
    {
        string Format(TimeSpan? time);
    }

    public static class TimeFormatConstants
    {
        public static readonly string MINUS = "−";
        public static readonly string DASH = "-";
    }
}
