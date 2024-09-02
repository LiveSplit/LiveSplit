using System;
using System.Collections.Generic;
using System.Linq;

using LiveSplit.Options;

using static System.Math;

namespace LiveSplit.Model.Comparisons;

public class MedianSegmentsComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public const string ComparisonName = "Median Segments"; //you win glacials
    public const string ShortComparisonName = "Median";
    public const double Weight = 0.75;

    public string Name => ComparisonName;

    public MedianSegmentsComparisonGenerator(IRun run)
    {
        Run = run;
    }

    protected TimeSpan CalculateMedian(IEnumerable<TimeSpan> curList)
    {
        int elementCount = curList.Count();
        var weightedList = curList.Select((x, i) => new KeyValuePair<double, TimeSpan>(GetWeight(i, elementCount), x)).ToList();
        weightedList = [.. weightedList.OrderBy(x => x.Value)];
        double totalWeights = weightedList.Aggregate(0.0, (s, x) => s + x.Key);
        double halfTotalWeights = totalWeights / 2;
        double curTotal = 0.0;
        foreach (KeyValuePair<double, TimeSpan> element in weightedList)
        {
            curTotal += element.Key;
            if (curTotal >= halfTotalWeights)
            {
                return element.Value;
            }
        }

        return TimeSpan.Zero;
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
                totalTime += CalculateMedian(curList);
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
