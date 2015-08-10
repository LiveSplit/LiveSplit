using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    public sealed class TimeStamp
    {
        private struct AccumulatedTime
        {
            public TimeSpan QPC;
            public TimeSpan NIST;
        }

        private static Stopwatch qpc;
        private static AccumulatedTime persistentAccumulatedTime { get; set; }
        private static AccumulatedTime accumulatedTime { get; set; }
        private static double Drift
        {
            get
            {
                if (persistentAccumulatedTime.NIST.TotalMilliseconds == 0)
                    return 1;

                return persistentAccumulatedTime.QPC.TotalMilliseconds
                    / persistentAccumulatedTime.NIST.TotalMilliseconds;
            }
        }

        private static TimeSpan firstQPCTime;
        private static DateTime firstNISTTime;

        private readonly TimeSpan value;

        private TimeStamp(TimeSpan value)
        {
            this.value = value;
        }

        static TimeStamp()
        {
            persistentAccumulatedTime = new AccumulatedTime
            {
                NIST = TimeSpan.Zero,
                QPC = TimeSpan.Zero
            };
            accumulatedTime = new AccumulatedTime
            {
                NIST = TimeSpan.Zero,
                QPC = TimeSpan.Zero
            };

            firstQPCTime = TimeSpan.Zero;
            firstNISTTime = DateTime.MinValue;

            qpc = new Stopwatch();
            qpc.Start();

            Task.Factory.StartNew(() => RefreshDrift());
        }

        public static TimeStamp Now
        {
            get
            {
                return new TimeStamp(qpc.Elapsed);
            }
        }

        private static void RefreshDrift()
        {
            //Thread.Sleep(TimeSpan.FromHours(3));
            while (true)
            {
                try
                {
                    var nistTime = GetNISTDate();
                    var qpcTime = qpc.Elapsed;
                    if (firstQPCTime != TimeSpan.Zero)
                    {
                        var qpcDelta = qpcTime - firstQPCTime;
                        var nistDelta = nistTime - firstNISTTime;

                        var newAccumulatedTime = accumulatedTime;
                        newAccumulatedTime.QPC = persistentAccumulatedTime.QPC + qpcDelta;
                        newAccumulatedTime.NIST = persistentAccumulatedTime.NIST + nistDelta;

                        accumulatedTime = newAccumulatedTime;

                        Debug.WriteLine(qpcDelta.TotalMilliseconds / nistDelta.TotalMilliseconds);
                    }
                    else
                    {
                        firstQPCTime = qpcTime;
                        firstNISTTime = nistTime;
                    }
                }
                catch { }
                Thread.Sleep(TimeSpan.FromMinutes(0.5));
            }
        }

        private static DateTime GetNISTDate()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
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
                var before = Now;
                TimeStamp after;

                // Open a StreamReader to a random time server
                using (var tcpClient = new TcpClient(servers[ran.Next(0, servers.Length)], 13))
                using (var reader = new StreamReader(tcpClient.GetStream()))
                {
                    after = Now;
                    serverResponse = reader.ReadToEnd();
                }

                var delta = TimeSpan.FromMilliseconds((after - before).TotalMilliseconds / 2);

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

                    return new DateTime(yr, mo, dy, hr, mm, sc) + delta;
                }
            }
            throw new Exception();
        }

        public static TimeSpan operator -(TimeStamp a, TimeStamp b)
        {
            return TimeSpan.FromMilliseconds((a.value.TotalMilliseconds - b.value.TotalMilliseconds) / Drift);
        }

        public static TimeStamp operator -(TimeStamp a, TimeSpan b)
        {
            return new TimeStamp(a.value - b);
        }
    }
}
