using LiveSplit.Options;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public class PercentileComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Balanced PB";
        public const string ShortComparisonName = "Balanced";
        public string Name { get { return ComparisonName; } }
        public const double Weight = 0.95;

        public PercentileComparisonGenerator(IRun run)
        {
            Run = run;
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
            for (var ind = 1; ind <= Run.AttemptHistory.Count; ind++)
            {
                var ignoreNextHistory = false;
                foreach (var segment in Run)
                {
                    IIndexedTime history = segment.SegmentHistory.FirstOrDefault(x => x.Index == ind);
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

            var weightedLists = new List<List<KeyValuePair<double, TimeSpan>>>();
            var forceMedian = false;

            foreach (var curList in allHistory)
            {
                if (curList.Count == 0)
                {
                    forceMedian = true;
                    break;
                }
                var tempList = curList.Select((x, i) => new KeyValuePair<double, TimeSpan>(GetWeight(i, curList.Count), x)).ToList();
                var weightedList = new List<KeyValuePair<double, TimeSpan>>();
                if (tempList.Count > 1)
                {
                    tempList = tempList.OrderBy(x => x.Value).ToList();
                    var totalWeight = tempList.Aggregate(0.0, (s, x) => (s + x.Key));
                    var smallestWeight = tempList[0].Key;
                    var rangeWeight = totalWeight - smallestWeight;
                    var aggWeight = 0.0;
                    foreach (var value in tempList)
                    {
                        aggWeight += value.Key;
                        weightedList.Add(new KeyValuePair<double, TimeSpan>((aggWeight - smallestWeight) / rangeWeight, value.Value));
                    }
                    weightedList = weightedList.OrderBy(x => x.Value).ToList();
                }
                else weightedList.Add(new KeyValuePair<double, TimeSpan>(1.0, tempList[0].Value));
                weightedLists.Add(weightedList);
            }

            var goalTime = TimeSpan.Zero;
            if (Run[Run.Count - 1].PersonalBestSplitTime[method].HasValue)
                goalTime = Run[Run.Count - 1].PersonalBestSplitTime[method].Value;

            var runSum = TimeSpan.Zero;
            var outputSplits = new List<TimeSpan>();
            var percentile = 0.5;
            var percMax = 1.0;
            var percMin = 0.0;
            var loopProtection = 0;

            do
            {
                runSum = TimeSpan.Zero;
                outputSplits.Clear();
                percentile = 0.5 * (percMax - percMin) + percMin;
                foreach (var weightedList in weightedLists)
                {
                    var curValue = TimeSpan.Zero;
                    if (weightedList.Count > 1)
                    {
                        for (var n = 0; n < weightedList.Count; n++)
                        {
                            if (weightedList[n].Key == percentile)
                            {
                                curValue = weightedList[n].Value;
                                break;
                            }
                            if (weightedList[n].Key > percentile)
                            {
                                var mult = 1 / (weightedList[n].Key - weightedList[n - 1].Key);
                                var percDn = (weightedList[n].Key - percentile) * mult * weightedList[n - 1].Value.Ticks;
                                var percUp = (percentile - weightedList[n - 1].Key) * mult * weightedList[n].Value.Ticks;
                                curValue = TimeSpan.FromTicks(Convert.ToInt64(percUp + percDn));
                                break;
                            }
                        }
                    }
                    else
                    {
                        curValue = weightedList[0].Value;
                    }
                    outputSplits.Add(curValue);
                    runSum += curValue;
                }
                if (runSum > goalTime)
                    percMax = percentile;
                else percMin = percentile;
                loopProtection += 1;
            } while (!(runSum - goalTime).Equals(TimeSpan.Zero) && loopProtection < 50 && !forceMedian);

            TimeSpan? totalTime = TimeSpan.Zero;
            for (var ind = 0; ind < Run.Count; ind++)
            {
                if (ind >= outputSplits.Count)
                    totalTime = null;
                if (totalTime != null)
                    totalTime += outputSplits[ind];
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
