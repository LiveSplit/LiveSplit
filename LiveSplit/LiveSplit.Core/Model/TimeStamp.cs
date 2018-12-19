using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    public sealed class TimeStamp
    {
        public static double PersistentDrift { get; set; }
        public static double NewDrift { get; set; }

        private static Stopwatch qpc;

        private static TimeSpan firstQPCTime;
        private static DateTime firstNTPTime;
        private static TimeSpan lastQPCTime;
        private static DateTime lastNTPTime;

        private readonly TimeSpan value;

        private TimeStamp(TimeSpan value)
        {
            this.value = value;
        }

        static TimeStamp()
        {
            PersistentDrift = 1.0;
            NewDrift = 1.0;

            firstQPCTime = lastQPCTime = TimeSpan.Zero;
            firstNTPTime = lastNTPTime = DateTime.MinValue;

            qpc = new Stopwatch();
            qpc.Start();

            Task.Factory.StartNew(() => RefreshDrift());
        }

        public static TimeStamp Now
            => new TimeStamp(TimeSpan.FromMilliseconds(qpc.Elapsed.TotalMilliseconds / PersistentDrift));
        
        public static bool IsSyncedWithAtomicClock
            => lastQPCTime != TimeSpan.Zero;

        public static AtomicDateTime CurrentDateTime
        {
            get
            {
                if (IsSyncedWithAtomicClock)
                {
                    return new AtomicDateTime(lastNTPTime.Add(Now - new TimeStamp(lastQPCTime)), true);
                }
                return new AtomicDateTime(DateTime.UtcNow, false);
            }
        }

        private static void RefreshDrift()
        {
            while (true)
            {
                var times = new List<long>();
                DateTime ntpTime;
                TimeSpan qpcTime = TimeSpan.Zero;
                for (var count = 1; count <= 10; count++)
                {
                    try
                    {
                        ntpTime = NTP.Now;
                        qpcTime = qpc.Elapsed;
                        times.Add(ntpTime.Ticks - qpcTime.Ticks);
                    }
                    catch { }
                    if (count < 10)
                        Wait(TimeSpan.FromSeconds(5));
                }
                if (times.Count >= 5)
                {
                    var averageDifference = times.Average();
                    lastQPCTime = qpcTime;
                    lastNTPTime = new DateTime(qpcTime.Ticks + (long)averageDifference, DateTimeKind.Utc);

                    if (firstQPCTime != TimeSpan.Zero)
                    {
                        var qpcDelta = lastQPCTime - firstQPCTime;
                        var ntpDelta = lastNTPTime - firstNTPTime;

                        var newDrift = qpcDelta.TotalMilliseconds / ntpDelta.TotalMilliseconds;
                        var weight = Math.Pow(0.95, ntpDelta.TotalHours);
                        NewDrift = Math.Pow(newDrift, 1 - weight) * Math.Pow(PersistentDrift, weight);

                        Wait(TimeSpan.FromHours(0.5));
                    }
                    else
                    {
                        firstQPCTime = lastQPCTime;
                        firstNTPTime = lastNTPTime;
                        Wait(TimeSpan.FromHours(1));
                    }
                }
                else Wait(TimeSpan.FromHours(0.5));
            }
        }

        private static void Wait(TimeSpan waitTime)
        {
            var before = Now;
            Thread.Sleep(waitTime);
            var elapsed = Now - before;
            if (elapsed.TotalMinutes > waitTime.TotalMinutes + 2)
            {
                firstQPCTime = TimeSpan.Zero;
                firstNTPTime = DateTime.MinValue;
            }
        }

        public static TimeSpan operator -(TimeStamp a, TimeStamp b)
            => a.value - b.value;

        public static TimeStamp operator -(TimeStamp a, TimeSpan b)
            => new TimeStamp(a.value - b);
    }
}
