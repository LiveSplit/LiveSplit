using System;

namespace LiveSplit.Tests.Model
{
    internal class Constants
    {
        public static readonly DateTime AnyDateTime = new DateTime(2020, 12, 13, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime AnotherDateTime = new DateTime(2020, 12, 4, 0, 0, 0, DateTimeKind.Utc);
        public const long DateTimeDifference = 7776000000000;
        public const long AnyTickValue = 7776000000000;
        public const long YetAnotherTickValue = 7760000000000;
        public const string TimeSerializationAsString = "9.00:00:00 | 03:00:00";
        public static readonly TimeSpan AnyTimeSpan = TimeSpan.FromTicks(AnyTickValue);
        public static readonly TimeSpan AnotherTimeSpan = TimeSpan.FromHours(3);
        public static readonly TimeSpan YetAnotherTimeSpan = TimeSpan.FromTicks(YetAnotherTickValue);
        public const long Difference = 7668000000000;
        public const long Addition = 7884000000000;
    }
}
