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

        protected double ReWeight(double a, double b, double c)
        {
            return (a - b) / c;
        }

        protected double Calculate(double perc, double Value1, double Key1, double Value2, double Key2)
        {
            var mult = 1 / (Key1 - Key2);
            var percDn = (Key1 - perc) * mult * Value2;
            var percUp = (perc - Key2) * mult * Value1;
            return percUp + percDn;
        }

        public void Generate(TimingMethod method)
        {
            var allHistory = new List<List<double>>();
            foreach (var segment in Run)
                allHistory.Add(new List<double>());
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
                            allHistory[Run.IndexOf(segment)].Add(history.Time[method].Value.Ticks);
                        }
                        else ignoreNextHistory = false;
                    }
                    else ignoreNextHistory = false;
                }
            }

            var weightedLists = new List<List<KeyValuePair<double, double>>>();
            var forceMedian = false;

            foreach (var curList in allHistory)
            {
                if (curList.Count == 0)
                {
                    if (Run[allHistory.IndexOf(curList)].PersonalBestSplitTime[method].HasValue)
                    {
                        if (allHistory.IndexOf(curList) == 0)
                            curList.Add(Run[allHistory.IndexOf(curList)].PersonalBestSplitTime[method].Value.Ticks);
                        else
                        {
                            if (Run[allHistory.IndexOf(curList) - 1].PersonalBestSplitTime[method].HasValue)
                                curList.Add(Run[allHistory.IndexOf(curList)].PersonalBestSplitTime[method].Value.Ticks - Run[allHistory.IndexOf(curList) - 1].PersonalBestSplitTime[method].Value.Ticks);
                            else
                            {
                                forceMedian = true;
                                break;
                            }

                        }
                    }
                    else
                    {
                        forceMedian = true;
                        break;
                    }
                }
                var tempList = curList.Select((x, i) => new KeyValuePair<double, double>(GetWeight(i, curList.Count), x)).ToList();
                var weightedList = new List<KeyValuePair<double, double>>();
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
                        weightedList.Add(new KeyValuePair<double, double>(ReWeight(aggWeight, smallestWeight, rangeWeight), value.Value));
                    }
                    weightedList = weightedList.OrderBy(x => x.Value).ToList();
                }
                else weightedList.Add(new KeyValuePair<double, double>(1.0, tempList[0].Value));
                weightedLists.Add(weightedList);
            }

            var goalTime = TimeSpan.Zero;
            if (Run[Run.Count - 1].PersonalBestSplitTime[method].HasValue)
                goalTime = Run[Run.Count - 1].PersonalBestSplitTime[method].Value;

            var runSum = TimeSpan.Zero;
            var outputSplits = new List<double>();
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
                    var curValue = 0.0;
                    if (weightedList.Count > 1)
                    {
                        for (var n = 0; n < weightedList.Count; n++)
                        {
                            if (weightedList[n].Key > percentile)
                            {
                                curValue = Calculate(percentile, weightedList[n].Value, weightedList[n].Key, weightedList[n - 1].Value, weightedList[n - 1].Key);
                                break;
                            }
                            if (weightedList[n].Key == percentile)
                            {
                                curValue = weightedList[n].Value;
                                break;
                            }
                        }
                    }
                    else
                    {
                        curValue = weightedList[0].Value;
                    }
                    outputSplits.Add(curValue);
                    runSum += TimeSpan.FromTicks(Convert.ToInt64(curValue));
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
                    totalTime += TimeSpan.FromTicks(Convert.ToInt64(outputSplits[ind]));
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
