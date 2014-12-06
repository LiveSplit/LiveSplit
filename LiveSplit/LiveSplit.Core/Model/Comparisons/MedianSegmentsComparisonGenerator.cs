using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.Model.Comparisons
{
    public class MedianSegmentsComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const String ComparisonName = "Median Segments"; //you win glacials
        public const String ShortComparisonName = "Median";
        public const double Weight = 0.75;

        public String Name { get { return ComparisonName; } }

        public MedianSegmentsComparisonGenerator(IRun run)
        {
            Run = run;
        }

        protected TimeSpan CalculateMedian(IEnumerable<TimeSpan> curList)
        {
            var elementCount = curList.Count();
            var weightedList = curList.Select((x, i) => new KeyValuePair<double, TimeSpan>(GetWeight(i, elementCount), x)).ToList();
            weightedList = weightedList.OrderBy(x => x.Value).ToList();
            var totalWeights = weightedList.Aggregate(0.0, (s, x) => (s + x.Key));
            var halfTotalWeights = totalWeights / 2;
            var curTotal = 0.0;
            foreach (var element in weightedList)
            {
                curTotal += element.Key;
                if (curTotal >= halfTotalWeights)
                    return element.Value;
            }
            return TimeSpan.Zero;
        }

        protected double GetWeight(int index, int count)
        {
            return Math.Pow(Weight, count - index - 1);
        }

        public void Generate(TimingMethod method)
        {
            var allHistory = new List<List<TimeSpan>>();
            foreach (var segment in Run)
                allHistory.Add(new List<TimeSpan>());
            for (var ind = 1; ind <= Run.RunHistory.Count; ind++)
            {
                var ignoreNextHistory = false;
                foreach (var segment in Run)
                {
                    IIndexedTime history;
                    history = segment.SegmentHistory.FirstOrDefault(x => x.Index == ind);
                    if (history != null)
                    {
                        if (history.Time[method] == null)
                            ignoreNextHistory = true;
                        else if (!ignoreNextHistory)
                        {
                            allHistory[Run.IndexOf(segment)].Add(history.Time[method].Value);
                        }
                        else ignoreNextHistory = false;
                    }
                    else break;
                }
            }
            TimeSpan? totalTime = TimeSpan.Zero;
            for (var ind = 0; ind < Run.Count; ind++)
            {
                var curList = allHistory[ind];
                if (curList.Count == 0)
                    totalTime = null;
                if (totalTime != null)
                    totalTime += CalculateMedian(curList);
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
}
