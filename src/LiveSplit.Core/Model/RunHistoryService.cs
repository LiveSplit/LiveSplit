using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.Model;

public record RunHistoryFilter(
    bool CompletedOnly = false,
    bool PbSegmentsOnly = false,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    TimingMethod Method = TimingMethod.RealTime
);

public static class RunHistoryService
{
    public static IList<Attempt> GetFilteredAttempts(IRun run, RunHistoryFilter filter)
    {
        var filtered = new List<Attempt>();

        foreach (Attempt attempt in run.AttemptHistory)
        {
            if (filter.CompletedOnly && attempt.Time[filter.Method] == null)
            {
                continue;
            }

            if (filter.PbSegmentsOnly && !HasPbSegments(run, attempt, filter.Method))
            {
                continue;
            }

            if (filter.FromDate.HasValue && (!attempt.Started.HasValue || attempt.Started.Value.Time < filter.FromDate.Value))
            {
                continue;
            }

            if (filter.ToDate.HasValue && (!attempt.Started.HasValue || attempt.Started.Value.Time > filter.ToDate.Value))
            {
                continue;
            }

            filtered.Add(attempt);
        }

        filtered.Sort((a, b) => b.Index.CompareTo(a.Index));
        return filtered;
    }

    public static IList<Attempt> GetPage(IList<Attempt> attempts, int page, int pageSize)
    {
        if (page < 1 || pageSize < 1)
        {
            return [];
        }

        int skip = (page - 1) * pageSize;
        if (skip >= attempts.Count)
        {
            return [];
        }

        return [.. attempts.Skip(skip).Take(pageSize)];
    }

    private static bool HasPbSegments(IRun run, Attempt attempt, TimingMethod method)
    {
        foreach (ISegment segment in run)
        {
            if (segment.SegmentHistory.TryGetValue(attempt.Index, out Time segmentTime))
            {
                TimeSpan? segmentTimeValue = segmentTime[method];
                TimeSpan? bestSegmentValue = segment.BestSegmentTime[method];

                if (segmentTimeValue != null && bestSegmentValue != null && segmentTimeValue == bestSegmentValue)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
