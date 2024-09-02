﻿using System;
using System.Diagnostics;

namespace LiveSplit.Options;

public static class Log
{
    static Log()
    {
        try
        {
            if (!EventLog.SourceExists("LiveSplit"))
            {
                EventLog.CreateEventSource("LiveSplit", "Application");
            }
        }
        catch { }

        try
        {
            var listener = new EventLogTraceListener("LiveSplit")
            {
                Filter = new EventTypeFilter(SourceLevels.Warning)
            };
            Trace.Listeners.Add(listener);
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

    public static void Error(string message)
    {
        try
        {
            Trace.TraceError(message);
        }
        catch { }
    }

    public static void Info(string message)
    {
        try
        {
            Trace.TraceInformation(message);
        }
        catch { }
    }

    public static void Warning(string message)
    {
        try
        {
            Trace.TraceWarning(message);
        }
        catch { }
    }
}
