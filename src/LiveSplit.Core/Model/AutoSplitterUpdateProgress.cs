using System;

namespace LiveSplit.Model;

public sealed class AutoSplitterUpdateProgress
{
    public double Percentage { get; }
    public string Status { get; }

    public AutoSplitterUpdateProgress(double percentage, string status)
    {
        Percentage = Math.Max(0, Math.Min(100, percentage));
        Status = status;
    }
}