using System;
using System.Diagnostics;

namespace LiveSplit.Model
{
    public class TripleDateTime
    {
        private static Stopwatch QPC;

        public TimeSpan QPCValue { get; protected set; }
        public double EnvironmentTickCount { get; protected set; }
        public DateTime UtcNow { get; protected set; }

        private TripleDateTime() { }

        static TripleDateTime()
        {
            QPC = new Stopwatch();
            QPC.Start();
        }

        public static TripleDateTime Now
        {
            get
            {
                return new TripleDateTime()
                {
                    QPCValue = QPC.Elapsed,
                    EnvironmentTickCount = Environment.TickCount,
                    UtcNow = DateTime.UtcNow
                };
            }
        }

        private static TimeSpan fromMs(double ms)
        {
            return new TimeSpan((long)(10000 * ms + 0.5));
        }

        public static TimeSpan operator -(TripleDateTime a, TripleDateTime b)
        {
            var ms1 = a.EnvironmentTickCount - b.EnvironmentTickCount;
            var timeSpan1 = fromMs(ms1);

            var timeSpan2 = a.QPCValue - b.QPCValue;
            var ms2 = timeSpan2.TotalMilliseconds;

            var timeSpan3 = a.UtcNow - b.UtcNow;
            var ms3 = timeSpan3.TotalMilliseconds;

            var delta12 = Math.Abs(ms1 - ms2);
            var delta23 = Math.Abs(ms2 - ms3);
            var delta31 = Math.Abs(ms3 - ms1);

            if (delta12 < delta23)
            {
                if (delta12 < delta31)
                    return fromMs((ms1 + ms2) / 2);
            }
            else if (delta23 < delta31)
                return fromMs((ms2 + ms3) / 2);
            return fromMs((ms3 + ms1) / 2);
        }

        public static TripleDateTime operator -(TripleDateTime a, TimeSpan b)
        {
            var ms = b.TotalMilliseconds;

            return new TripleDateTime()
            {
                EnvironmentTickCount = a.EnvironmentTickCount - ms,
                QPCValue = a.QPCValue - b,
                UtcNow = a.UtcNow - b
            };
        }

        public static TripleDateTime operator +(TripleDateTime a, TimeSpan b)
        {
            var ms = b.TotalMilliseconds;

            return new TripleDateTime()
            {
                EnvironmentTickCount = a.EnvironmentTickCount + ms,
                QPCValue = a.QPCValue + b,
                UtcNow = a.UtcNow + b
            };

        }
    }
}
