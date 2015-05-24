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

        public void Generate(TimingMethod method) //Starting here
        {
            var allHistory = new List<List<double>>(); //Begin the listception
            foreach (var segment in Run)
                allHistory.Add(new List<double>()); //A list added for every segment
            for (var ind = 1; ind <= Run.AttemptHistory.Count; ind++) //For each attempt
            {
                var ignoreNextHistory = false; //This is set because of how LiveSplit saves segments. If a split is skipped, it's lumped into the next saved split. We need to remove those.
                foreach (var segment in Run) //For each segment
                {
                    IIndexedTime history; //Makes a Time and Index value
                    history = segment.SegmentHistory.FirstOrDefault(x => x.Index == ind); //If the attempt has any saved splits, returns the index and time of the current attempt's segment, otherwise null.
                    if (history != null) //If there is at least one split saved for the attempt...
                    {
                        if (history.Time[method] == null) //If there's no recorded time for the current attempt's segment...
                            ignoreNextHistory = true; //Forget the current and next attempt's segment.
                        else if (!ignoreNextHistory) //If the previous attempt's segment was not empty...
                        {
                            allHistory[Run.IndexOf(segment)].Add(history.Time[method].Value.TotalMilliseconds); //Add the time of the current attempt's segment to the segment's list.
                        }
                        else ignoreNextHistory = false; //Forget the current attempt's segment but allow the next one.
                    }
                    else break; //If no splits were saved, check next attempt.
                }
            } //So far we've made allHistory, which contains all the usable times from each attempt for each segment, by order of attempt.

            var weightedLists = new List<List<KeyValuePair<double,double>>>(); //This monster will be saved so the looping percentile function doesn't take as long.
            var forceMedian = false; //If any of the segments are empty, we'll force the percentile to be 0.5.

            foreach (var curList in allHistory) //For each segment's list
            {
                if (curList.Count == 0) //If the segment is empty, it's pointless to continue.
                {
                    forceMedian = true; //Not every segment will have a split, so the calculation will be a median instead.
                    break;
                }
                var tempList = curList.Select((x, i) => new KeyValuePair<double, double>(Math.Pow(Weight, curList.Count - i - 1), x)).ToList(); //Create the weighted list for the segment
                var weightedList = new List<KeyValuePair<double, double>>(); //Ready the adjusted weighted list
                if (tempList.Count > 1) //There has to be more than one saved time to be adjusted
                {
                    tempList = tempList.OrderBy(x => x.Value).ToList(); //Sort it by time ascending
                    var totalWeight = tempList.Aggregate(0.0, (s, x) => (s + x.Key)); //Get the sum of the weights
                    var smallestWeight = tempList[0].Key; //Get the weight with the lowest value (right?)
                    var aggWeight = 0.0;
                    foreach (var value in tempList) //This adjusts the list so that the smallest weight is 0.0 and the largest is 1.0.
                    {
                        aggWeight += value.Key;
                        weightedList.Add(new KeyValuePair<double, double>((aggWeight - smallestWeight) / (totalWeight - smallestWeight), value.Value));
                    }
                    weightedList = weightedList.OrderBy(x => x.Value).ToList(); //Sort it by time ascending
                }
                else weightedList.Add(new KeyValuePair<double, double>(1.0, tempList[0].Value)); //If only one split saved, just use that.
                weightedLists.Add(weightedList); //Save them to the master list
            } //Now we have each segment's list weighted and sorted.

            var goalTime = 0.0; //Fetch and store the Personal Best time. If there is no PB, leave it at 0.
            if (Run[Run.Count - 1].PersonalBestSplitTime[method].HasValue)
                goalTime = Run[Run.Count - 1].PersonalBestSplitTime[method].Value.TotalMilliseconds;

            List<double> outputSplits = new List<double>(); //Where the percentile function will save the generated splits.
            var percentile = 0.5; //Start at 0.5 as a 'binary attack' to determine the best position.
            var percMin = 0.0; //Lowest value the percentile can go.
            var percMax = 1.0; //Highest value the percentile can go.

            do
            {
                var runSum = 0.0; //Summing the generated times to compare check with the PB.
                foreach (var weightedList in weightedLists) //Going through each weighted list
                {
                    var curValue = 0.0; //Value to be saved into the OutputSplits and to add up to RumSum.
                    if (weightedList.Count > 1) //If there's only one split then just use that one.
                    {
                        for (var n = 0; n < weightedList.Count; n++) //Going through the values of the list
                        {
                            if (weightedList[n].Key == percentile) //Very rare but here incase
                            {
                                curValue = weightedList[n].Value;
                                break;
                            }
                            if (weightedList[n].Key > percentile) //Once the key larger than the percentile is found, use that value and the previous one to form the output split.
                            {
                                var mult = 1 / (weightedList[n].Key - weightedList[n - 1].Key);
                                var percDn = (weightedList[n].Key - percentile) * mult * weightedList[n - 1].Value;
                                var percUp = (percentile - weightedList[n - 1].Key) * mult * weightedList[n].Value;
                                curValue = percUp + percDn;
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

                if (runSum == goalTime || percentile < 0.0000000001 || percMax - percMin < 0.0000000001 || forceMedian == true)
                    break; //Upon satisfaction and to prevent looping indefinitally
                else if (runSum > goalTime)
                    percMax = percentile; //If the RunSum is too high, lower the ceiling
                else percMin = percentile; //If the RunSum is too low, increase the floor
                outputSplits.Clear(); //Need to clear out the list before reusing it
                percentile = 0.5 * (percMax - percMin) + percMin; //Recalculate the percentile
            } while (true);

            TimeSpan? totalTime = TimeSpan.Zero;
            for (var ind = 0; ind < Run.Count; ind++)
            {
                if (ind >= outputSplits.Count)
                    totalTime = null;
                if (totalTime != null)
                    totalTime += TimeSpan.FromMilliseconds(outputSplits[ind]);
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
