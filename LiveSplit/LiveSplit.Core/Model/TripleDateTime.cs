using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    public class TripleDateTime
    {
        private static Stopwatch QPC;
        private static Stopwatch NISTQPC;

        private static DateTime FirstNIST;
        public static TimeSpan NISTQPCOffset { get; protected set; }

        private static System.Timers.Timer SynchronizationTimer;

        public static double SynchronizationInterval
        {
            get
            {
                return SynchronizationTimer.Interval;
            }
            set
            {
                SynchronizationTimer.Interval = value;
            }
        }
        public static bool SynchronizationEnabled
        {
            get
            {
                return SynchronizationTimer.Enabled;
            }
            set
            {
                SynchronizationTimer.Enabled = value;
            }
        }

        public TimeSpan QPCValue { get; protected set; }
        public double EnvironmentTickCount { get; protected set; }
        public DateTime UtcNow { get; protected set; }

        private TripleDateTime() { }

        static TripleDateTime()
        {
            QPC = new Stopwatch();
            QPC.Start();

            /*FirstNIST = GetNISTDate();
            NISTQPC = new Stopwatch();
            NISTQPC.Start();

            SynchronizationTimer = new System.Timers.Timer(10 * 60 * 1000) { Enabled = false };
            SynchronizationTimer.Elapsed += (s, e) =>
            {
                var nistQPC = NISTQPC.Elapsed;
                var nist = GetNISTDate();

                var nistDelta = nist - FirstNIST;
                var nistQPCDelta = nistQPC;

                NISTQPCOffset = TimeSpan.FromSeconds((int)(nistDelta.TotalSeconds - nistQPCDelta.TotalSeconds));
            };*/
        }

        public static TripleDateTime Now
        {
            get
            {
                return new TripleDateTime()
                {
                    QPCValue = QPC.Elapsed + NISTQPCOffset,
                    EnvironmentTickCount = Environment.TickCount + NISTQPCOffset.TotalMilliseconds,
                    UtcNow = DateTime.UtcNow + NISTQPCOffset
                };
            }
        }

        private static DateTime GetNISTDate()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            DateTime date = new DateTime(1000, 1, 1);
            string serverResponse = string.Empty;

            // Represents the list of NIST servers
            string[] servers = new string[] {
                /*"nist1-ny.ustiming.org",
                "time-a.nist.gov",
                "nist1-chi.ustiming.org",*/
                "time.nist.gov",
                /*"ntp-nist.ldsbc.edu",
                "nist1-la.ustiming.org"       */                  
            };

            // Try each server in random order to avoid blocked requests due to too frequent request
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    // Open a StreamReader to a random time server
                    StreamReader reader = new StreamReader(new System.Net.Sockets.TcpClient(servers[ran.Next(0, servers.Length)], 13).GetStream());
                    serverResponse = reader.ReadToEnd();
                    reader.Close();

                    // Check to see that the signature is there
                    if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
                    {
                        // Parse the date
                        int jd = int.Parse(serverResponse.Substring(1, 5));
                        int yr = int.Parse(serverResponse.Substring(7, 2));
                        int mo = int.Parse(serverResponse.Substring(10, 2));
                        int dy = int.Parse(serverResponse.Substring(13, 2));
                        int hr = int.Parse(serverResponse.Substring(16, 2));
                        int mm = int.Parse(serverResponse.Substring(19, 2));
                        int sc = int.Parse(serverResponse.Substring(22, 2));

                        if (jd > 51544)
                            yr += 2000;
                        else
                            yr += 1999;

                        date = new DateTime(yr, mo, dy, hr, mm, sc);

                        // Exit the loop
                        break;
                    }
                }
                catch (Exception ex)
                {
                    /* Do Nothing...try the next server */
                }
            }
            return date;
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
