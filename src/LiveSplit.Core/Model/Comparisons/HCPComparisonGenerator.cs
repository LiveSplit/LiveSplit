using System;
using System.Collections.Generic;
using System.Linq;

using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons;

public class HCPComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public const string ComparisonName = "Golf HCP";
    public const string ShortComparisonName = "HCP";
    public string Name => ComparisonName;
    private const int MaximumNumberOfBestRunsToIncludePerSegment = 8;
    private const int NumberOfLatestRunsToInclude = 20;

    public HCPComparisonGenerator(IRun run)
    {
        Run = run;
    }

    protected TimeSpan CalculateAverage(IEnumerable<TimeSpan> curList)
    {
        double averageTime = curList.OrderBy(x => x.TotalSeconds).Take(MaximumNumberOfBestRunsToIncludePerSegment).Average(x => x.TotalSeconds);
        return TimeSpan.FromTicks((long)(averageTime * TimeSpan.TicksPerSecond));
    }

    public void Generate(TimingMethod method)
    {
        int segmentCount = Run.Count;
        var recentRuns = Run.AttemptHistory
            .Where(attempt => Run.Last().SegmentHistory.TryGetValue(attempt.Index, out var lastSegmentTime) && lastSegmentTime[method] != null)
            .OrderByDescending(attempt => attempt.Index)
            .Take(NumberOfLatestRunsToInclude)
            .ToList();

        if (recentRuns.Count == 0)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                var time = new Time(Run[i].Comparisons[Name]);
                time[method] = null;
                Run[i].Comparisons[Name] = time;
            }
            return;
        }

        // Gather total times and per-segment times for each run
        var totalTimes = new List<double>();
        var segmentTimesPerRun = new List<List<double>>();
        foreach (var attempt in recentRuns)
        {
            var segmentTimes = new List<double>();
            double? lastCumulativeTime = 0;
            bool valid = true;
            for (int i = 0; i < segmentCount; i++)
            {
                if (Run[i].SegmentHistory.TryGetValue(attempt.Index, out var segmentHistory) && segmentHistory[method] != null)
                {
                    segmentTimes.Add(segmentHistory[method].Value.TotalSeconds);
                    lastCumulativeTime += segmentHistory[method].Value.TotalSeconds;
                }
                else
                {
                    valid = false;
                    break;
                }
            }
            if (valid && lastCumulativeTime.HasValue)
            {
                totalTimes.Add(lastCumulativeTime.Value);
                segmentTimesPerRun.Add(segmentTimes);
            }
        }

        if (totalTimes.Count == 0)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                var time = new Time(Run[i].Comparisons[Name]);
                time[method] = null;
                Run[i].Comparisons[Name] = time;
            }
            return;
        }

        double avgTotal = totalTimes.Average();

        // For each segment, compute average proportion of total time
        var avgProportions = new double[segmentCount];
        for (int seg = 0; seg < segmentCount; seg++)
        {
            double sum = 0;
            int count = 0;
            for (int run = 0; run < segmentTimesPerRun.Count; run++)
            {
                double segTime = segmentTimesPerRun[run][seg];
                double total = totalTimes[run];
                if (total > 0)
                {
                    sum += segTime / total;
                    count++;
                }
            }
            avgProportions[seg] = count > 0 ? sum / count : 1.0 / segmentCount;
        }

        // Assign cumulative times to segments
        TimeSpan? cumulative = TimeSpan.Zero;
        for (int i = 0; i < segmentCount; i++)
        {
            var segmentDuration = TimeSpan.FromSeconds(avgProportions[i] * avgTotal);
            if (cumulative != null)
            {
                cumulative += segmentDuration;
            }  

            var time = new Time(Run[i].Comparisons[Name]);
            time[method] = cumulative;
            Run[i].Comparisons[Name] = time;
        }
    }

    public void Generate(ISettings settings)
    {
        Generate(TimingMethod.RealTime);
        Generate(TimingMethod.GameTime);
    }
}
