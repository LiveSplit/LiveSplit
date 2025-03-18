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
        var allHistory = new List<List<TimeSpan>>();
        foreach (ISegment segment in Run)
        {
            allHistory.Add([]);
        }

        // Get the last 20 complete runs 
        var recentRuns = Run.AttemptHistory
            .Where(attempt => Run.Last().SegmentHistory.TryGetValue(attempt.Index, out Time lastSegmentTime) && lastSegmentTime[method] != null)
            .OrderByDescending(attempt => attempt.Index)
            .Take(NumberOfLatestRunsToInclude);

        foreach (Attempt attempt in recentRuns)
        {
            int ind = attempt.Index;
            bool ignoreNextHistory = false;
            foreach (ISegment segment in Run)
            {
                if (segment.SegmentHistory.TryGetValue(ind, out Time history))
                {
                    if (history[method] == null)
                    {
                        ignoreNextHistory = true;
                    }
                    else if (!ignoreNextHistory)
                    {
                        allHistory[Run.IndexOf(segment)].Add(history[method].Value);
                    }
                    else
                    {
                        ignoreNextHistory = false;
                    }
                }
                else
                {
                    ignoreNextHistory = false;
                }
            }
        }

        TimeSpan? totalTime = TimeSpan.Zero;
        for (int ind = 0; ind < Run.Count; ind++)
        {
            List<TimeSpan> curList = allHistory[ind];
            if (curList.Count == 0)
            {
                totalTime = null;
            }

            if (totalTime != null)
            {
                totalTime += CalculateAverage(curList);
            }

            var time = new Time(Run[ind].Comparisons[Name]);
            time[method] = totalTime;
            Run[ind].Comparisons[Name] = time;
        }
    }

    public void Generate(ISettings settings)
    {
        Generate(TimingMethod.RealTime);
        Generate(TimingMethod.GameTime);
    }
}
