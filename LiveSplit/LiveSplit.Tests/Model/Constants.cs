using System;

namespace LiveSplit.Tests.Model
{
    internal class Constants
    {
        public static readonly DateTime AnyDateTime = new DateTime(2020, 12, 13);
        public static readonly DateTime AnotherDateTime = new DateTime(2020, 12, 4);
        public const long DateTimeDifference = 7776000000000;
        public const long AnyTickValue = 7776000000000;
        public const string TimeSerializationAsString = "9.00:00:00 | 03:00:00";
        public static readonly TimeSpan AnyTimeSpan = TimeSpan.FromTicks(AnyTickValue);
        public static readonly TimeSpan AnotherTimeSpan = TimeSpan.FromHours(3);
        public static readonly TimeSpan YetAnotherTimeSpan = TimeSpan.FromMinutes(1774);
        public const long Difference = 7668000000000;
        public const long Addition = 7884000000000;
    }
}
