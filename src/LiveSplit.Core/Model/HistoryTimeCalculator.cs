using System;
using System.Collections.Generic;

namespace LiveSplit.Model;

public static class HistoryTimeCalculator
{
    public static IList<TimeSpan?> GetSegmentTimesForAttempt(IRun run, int attemptIndex, TimingMethod method)
    {
        var segmentTimes = new List<TimeSpan?>(run.Count);
        for (int i = 0; i < run.Count; i++)
        {
            if (run[i].SegmentHistory.TryGetValue(attemptIndex, out Time segTime))
            {
                segmentTimes.Add(segTime[method]);
            }
            else
            {
                segmentTimes.Add(null);
            }
        }

        return segmentTimes;
    }

    public static IList<TimeSpan?> GetSplitTimesForAttempt(IRun run, int attemptIndex, TimingMethod method)
    {
        var splitTimes = new List<TimeSpan?>(run.Count);
        TimeSpan? total = TimeSpan.Zero;

        for (int i = 0; i < run.Count; i++)
        {
            TimeSpan? segmentTime;
            if (run[i].SegmentHistory.TryGetValue(attemptIndex, out Time segHistTime))
            {
                segmentTime = segHistTime[method];
                if (segmentTime == null)
                {
                    total = null;
                }
            }
            else
            {
                segmentTime = null;
                total = null;
            }

            if (total != null)
            {
                total += segmentTime;
                splitTimes.Add(total);
            }
            else
            {
                splitTimes.Add(null);
            }
        }

        return splitTimes;
    }

    public static IList<TimeSpan?> GetSegmentBestDiffsForAttempt(IRun run, int attemptIndex, TimingMethod method)
    {
        var segmentTimes = GetSegmentTimesForAttempt(run, attemptIndex, method);
        var diffs = new List<TimeSpan?>(run.Count);

        for (int i = 0; i < run.Count; i++)
        {
            TimeSpan? bestSegmentTime = run[i].BestSegmentTime[method];
            TimeSpan? diff = null;

            if (segmentTimes[i] != null && bestSegmentTime != null)
            {
                diff = segmentTimes[i] - bestSegmentTime;
            }

            diffs.Add(diff);
        }

        return diffs;
    }
}
