using System;
using System.Diagnostics;

namespace LiveSplit.Options
{
    public static class Log
    {
        static Log()
        {
            try
            {
                if (!EventLog.SourceExists("LiveSplit"))
                    EventLog.CreateEventSource("LiveSplit", "Application");
            }
            catch { }

            try
            {
                Trace.Listeners.Add(new EventLogTraceListener("LiveSplit"));
            }
            catch { }
        }

        public static void Error(Exception ex)
        {
            try
            {
                Trace.TraceError("{0}\n\n{1}", ex.Message, ex.StackTrace);
            }
            catch { }
        }

        public static void Error(String message)
        {
            try
            {
                Trace.TraceError(message);
            }
            catch { }
        }

        public static void Info(String message)
        {
            try
            {
                Trace.TraceInformation(message);
            }
            catch { }
        }

        public static void Warning(String message)
        {
            try
            {
                Trace.TraceWarning(message);
            }
            catch { }
        }
    }
}
