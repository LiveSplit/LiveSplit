using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace LiveSplit.Model.Comparisons;

public class SegmentPercentileComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public const string ComparisonName = "Percentile Segments";
    public const string ShortComparisonName = "Percentile";
    public const double Weight = 0.75;
    public const double DefaultPercentile = 33;

    public virtual string Name => ComparisonName;

    // Percentile in the range [0, 100]. 0 is the best segment, 50 the median, 100 the worst.
    public double Percentile { get; set; } = DefaultPercentile;

    public SegmentPercentileComparisonGenerator(IRun run)
    {
        Run = run;
    }

    protected TimeSpan CalculatePercentile(IEnumerable<TimeSpan> curList)
    {
        int elementCount = curList.Count();
        var weightedList = curList.Select((x, i) => new KeyValuePair<double, TimeSpan>(GetWeight(i, elementCount), x)).ToList();
        weightedList = [.. weightedList.OrderBy(x => x.Value)];
        double totalWeights = weightedList.Sum(x => x.Key);
        double percentile = Max(0, Min(100, Percentile));
        if (percentile == 100)
        {
            return weightedList[^1].Value;
        }

        double targetWeight = totalWeights * (percentile / 100);
        double curTotal = 0.0;
        foreach (KeyValuePair<double, TimeSpan> element in weightedList)
        {
            curTotal += element.Key;
            if (curTotal >= targetWeight)
            {
                return element.Value;
            }
        }

        return weightedList[^1].Value;
    }

    protected double GetWeight(int index, int count)
    {
        return Pow(Weight, count - index - 1);
    }

    public void Generate(TimingMethod method)
    {
        var allHistory = new List<List<TimeSpan>>();
        foreach (ISegment segment in Run)
        {
            allHistory.Add([]);
        }

        foreach (Attempt attempt in Run.AttemptHistory)
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
                    break;
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
                totalTime += CalculatePercentile(curList);
            }

            var time = new Time(Run[ind].Comparisons[Name]);
            time[method] = totalTime;
            Run[ind].Comparisons[Name] = time;
        }
    }

    public virtual void Generate(ISettings settings)
    {
        Percentile = settings.SegmentPercentile;
        Generate(TimingMethod.RealTime);
        Generate(TimingMethod.GameTime);
    }
}
