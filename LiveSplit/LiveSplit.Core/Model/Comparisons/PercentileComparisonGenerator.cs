using LiveSplit.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using static System.Math;

namespace LiveSplit.Model.Comparisons
{
    public class PercentileComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Balanced PB";
        public const string ShortComparisonName = "Balanced";
        public string Name => ComparisonName;
        public const double Weight = 0.9375;

        public PercentileComparisonGenerator(IRun run)
        {
            Run = run;
        }

        protected double GetWeight(int index, int count) => Pow(Weight, count - index - 1);

        protected double ReWeight(double a, double b, double c) => (a - b) / c;

        protected TimeSpan Calculate(double perc, TimeSpan Value1, double Key1, TimeSpan Value2, double Key2)
        {
            var percDn = (Key1 - perc) * Value2.Ticks / (Key1 - Key2);
            var percUp = (perc - Key2) * Value1.Ticks / (Key1 - Key2);
            return TimeSpan.FromTicks(Convert.ToInt64(percUp + percDn));
        }

        public void Generate(TimingMethod method)
        {
            var allHistory = new List<List<IndexedTimeSpan>>();
            foreach (var segment in Run)
                allHistory.Add(new List<IndexedTimeSpan>());
            foreach (var attempt in Run.AttemptHistory)
            {
                var ind = attempt.Index;
                var historyStartingIndex = -1;
                foreach (var segment in Run)
                {
                    var currentIndex = Run.IndexOf(segment);
                    Time history;
                    if (segment.SegmentHistory.TryGetValue(ind, out history))
                    {
                        if (history[method] != null)
                        {
                            allHistory[Run.IndexOf(segment)].Add(new IndexedTimeSpan(history[method].Value, historyStartingIndex));
                            historyStartingIndex = currentIndex;
                        }
                    }
                    else historyStartingIndex = currentIndex;
                }
            }

            var weightedLists = new List<List<KeyValuePair<double, TimeSpan>>>();
            var overallStartingIndex = -1;

            foreach (var currentList in allHistory)
            {
                var nullSegment = false;
                var curIndex = allHistory.IndexOf(currentList);
                var curPBTime = Run[curIndex].PersonalBestSplitTime[method];
                var previousPBTime = overallStartingIndex >= 0 ? Run[overallStartingIndex].PersonalBestSplitTime[method] : null;
                var finalList = new List<TimeSpan>();

                var matchingSegmentHistory = currentList.Where(x => x.Index == overallStartingIndex);
                if (matchingSegmentHistory.Any())
                {
                    finalList = matchingSegmentHistory.Select(x => x.Time).ToList();
                    overallStartingIndex = curIndex;
                }
                else if (curPBTime != null && previousPBTime != null)
                {
                    finalList.Add(curPBTime.Value - previousPBTime.Value);
                    overallStartingIndex = curIndex;
                }
                else
                {
                    nullSegment = true;
                }

                if (!nullSegment)
                {
                    var tempList = finalList.Select((x, i) => new KeyValuePair<double, TimeSpan>(GetWeight(i, finalList.Count), x)).ToList();
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
                            weightedList.Add(new KeyValuePair<double, TimeSpan>(ReWeight(aggWeight, smallestWeight, rangeWeight), value.Value));
                        }
                        weightedList = weightedList.OrderBy(x => x.Value).ToList();
                    }
                    else weightedList.Add(new KeyValuePair<double, TimeSpan>(1.0, tempList[0].Value));
                    weightedLists.Add(weightedList);
                }
                else
                    weightedLists.Add(null);
            }

            TimeSpan? goalTime = null;
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
                    if (weightedList != null)
                    {
                        var curValue = TimeSpan.Zero;
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
                        runSum += curValue;
                    }
                    else
                        outputSplits.Add(TimeSpan.Zero);
                }
                if (runSum > goalTime)
                    percMax = percentile;
                else percMin = percentile;
                loopProtection += 1;
            } while (!(runSum - goalTime).Equals(TimeSpan.Zero) && loopProtection < 50 && goalTime != null);

            TimeSpan totalTime = TimeSpan.Zero;
            TimeSpan? useTime = TimeSpan.Zero;
            for (var ind = 0; ind < Run.Count; ind++)
            {
                totalTime += outputSplits[ind];
                if (outputSplits[ind] == TimeSpan.Zero)
                    useTime = null;
                else
                    useTime = totalTime;
                var time = new Time(Run[ind].Comparisons[Name]);
                time[method] = useTime;
                Run[ind].Comparisons[Name] = time;
            }
        }

        public void Generate(ISettings settings)
        {
            Generate(TimingMethod.RealTime);
            Generate(TimingMethod.GameTime);
        }

        class IndexedTimeSpan
        {
            public TimeSpan Time { get; set; }
            public int Index { get; set; }

            public IndexedTimeSpan(TimeSpan time, int index)
            {
                Time = time;
                Index = index;
            }
        }
    }
}
