using System;

namespace LiveSplit.Model
{
    public struct AtomicDateTime
    {
        public DateTime Time { get; }
        public bool SyncedWithAtomicClock { get; private set; }

        public AtomicDateTime(DateTime time, bool synced)
        {
            Time = time;
            SyncedWithAtomicClock = synced;
        }

        public static TimeSpan operator -(AtomicDateTime a, AtomicDateTime b)
            => a.Time - b.Time;

        public static TimeSpan operator -(AtomicDateTime a, DateTime b)
            => a.Time - b;
    }
}
