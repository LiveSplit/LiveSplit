using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
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
            while (true)
            {
                var times = new List<long>();
                DateTime networkTime;
                TimeSpan qpcTime = TimeSpan.Zero;
                for (var count = 1; count <= 10; count++)
                {
                    try
                    {
                        networkTime = GetNetworkTime();
                        qpcTime = qpc.Elapsed;
                        times.Add(networkTime.Ticks - qpcTime.Ticks);
                        Debug.WriteLine(qpcTime);
                    }
                    catch { }
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                if (times.Count > 0)
                {
                    var averageDifference = times.Average();
                    networkTime = new DateTime(qpcTime.Ticks + (long)averageDifference);

                    if (firstQPCTime != TimeSpan.Zero)
                    {
                        var qpcDelta = qpcTime - firstQPCTime;
                        var nistDelta = networkTime - firstNISTTime;

                        var newAccumulatedTime = accumulatedTime;
                        newAccumulatedTime.QPC = persistentAccumulatedTime.QPC + qpcDelta;
                        newAccumulatedTime.NIST = persistentAccumulatedTime.NIST + nistDelta;

                        accumulatedTime = newAccumulatedTime;

                        Debug.WriteLine(qpcDelta.TotalMilliseconds / nistDelta.TotalMilliseconds);
                        Thread.Sleep(TimeSpan.FromHours(0.5));
                    }
                    else
                    {
                        firstQPCTime = qpcTime;
                        firstNISTTime = networkTime;
                        Thread.Sleep(TimeSpan.FromHours(1));
                    }
                }
                else break;
            }
        }

        // stackoverflow.com/a/12150289
        public static DateTime GetNetworkTime()
        {
            var before = Now;
            TimeStamp after;

            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            after = Now;
            var delta = TimeSpan.FromMilliseconds((after - before).TotalMilliseconds / 2);

            return networkDateTime + delta;
        }

        // stackoverflow.com/a/3294698
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
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
