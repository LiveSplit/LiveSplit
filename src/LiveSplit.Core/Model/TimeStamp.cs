using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.Model;

public sealed class TimeStamp
{
    public static double PersistentDrift { get; set; }
    public static double NewDrift { get; set; }

    private static readonly Stopwatch qpc;

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

        Task.Factory.StartNew(RefreshDrift);
    }

    public static TimeStamp Now
        => new(TimeSpan.FromTicks((long)(qpc.Elapsed.Ticks / PersistentDrift)));

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
            for (int count = 1; count <= 10; count++)
            {
                try
                {
                    ntpTime = NTP.Now;
                    qpcTime = qpc.Elapsed;
                    times.Add(ntpTime.Ticks - qpcTime.Ticks);
                }
                catch { }

                if (count < 10)
                {
                    Wait(TimeSpan.FromSeconds(5));
                }
            }

            if (times.Count >= 5)
            {
                double averageDifference = times.Average();
                lastQPCTime = qpcTime;
                lastNTPTime = new DateTime(qpcTime.Ticks + (long)averageDifference, DateTimeKind.Utc);

                if (firstQPCTime != TimeSpan.Zero)
                {
                    TimeSpan qpcDelta = lastQPCTime - firstQPCTime;
                    TimeSpan ntpDelta = lastNTPTime - firstNTPTime;

                    double newDrift = qpcDelta.TotalMilliseconds / ntpDelta.TotalMilliseconds;

                    // Ignore any drift that is too far from 1
                    if (Math.Abs(newDrift - 1) < 0.01)
                    {
                        double weight = Math.Pow(0.95, ntpDelta.TotalHours);
                        NewDrift = Math.Pow(newDrift, 1 - weight) * Math.Pow(PersistentDrift, weight);
                    }

                    Wait(TimeSpan.FromHours(0.5));
                }
                else
                {
                    firstQPCTime = lastQPCTime;
                    firstNTPTime = lastNTPTime;
                    Wait(TimeSpan.FromHours(1));
                }
            }
            else
            {
                Wait(TimeSpan.FromHours(0.5));
            }
        }
    }

    private static void Wait(TimeSpan waitTime)
    {
        TimeStamp before = Now;
        Thread.Sleep(waitTime);
        TimeSpan elapsed = Now - before;
        if (elapsed.TotalMinutes > waitTime.TotalMinutes + 2)
        {
            firstQPCTime = TimeSpan.Zero;
            firstNTPTime = DateTime.MinValue;
        }
    }

    public static TimeSpan operator -(TimeStamp a, TimeStamp b)
    {
        return a.value - b.value;
    }

    public static TimeStamp operator -(TimeStamp a, TimeSpan b)
    {
        return new TimeStamp(a.value - b);
    }
}
