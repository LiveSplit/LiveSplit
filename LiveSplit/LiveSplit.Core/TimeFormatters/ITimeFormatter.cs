using System;

namespace LiveSplit.TimeFormatters
{
    public interface ITimeFormatter
    {
        string Format(TimeSpan? time);
    }

    public class TimeFormatConstants
    {
        public static string MINUS = "−";
        public static string DASH = "-";
    }
}
