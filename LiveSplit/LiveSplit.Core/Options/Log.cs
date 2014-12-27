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

            Trace.Listeners.Add(new EventLogTraceListener("LiveSplit"));
        }

        public static void Error(Exception ex)
        {
            if (ex == null)
                return;

            Trace.TraceError("{0}\n\n{1}", ex.Message, ex.StackTrace);
        }

        public static void Error(string message)
        {
            Trace.TraceError(message);
        }

        public static void Info(string message)
        {
            Trace.TraceInformation(message);
        }

        public static void Warning(string message)
        {
            Trace.TraceWarning(message);
        }
    }
}
