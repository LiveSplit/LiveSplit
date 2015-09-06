using System;
using System.Collections.Generic;
using System.Linq;
using static LiveSplit.Model.SumOfSegmentsHelper;

namespace LiveSplit.Model
{
    public static class SumOfBest
    {
        private static void PopulatePrediction(IList<TimeSpan?> predictions, TimeSpan? predictedTime, int segmentIndex)
        {
            if (predictedTime.HasValue && (!predictions[segmentIndex].HasValue || predictedTime < predictions[segmentIndex].Value))
            {
                predictions[segmentIndex] = predictedTime;
            }
        }

        private static void PopulatePredictions(IRun run, TimeSpan? currentTime, int segmentIndex, IList<TimeSpan?> predictions, bool simpleCalculation, bool useCurrentRun, TimingMethod method)
        {
            if (currentTime != null)
            {
                PopulatePrediction(predictions, currentTime + run[segmentIndex].BestSegmentTime[method], segmentIndex + 1);
                if (!simpleCalculation)
                {
                    foreach (var nullSegment in run[segmentIndex].SegmentHistory.Where(x => !x.Value[method].HasValue))
                    {
                        Time segmentTime;
                        if (segmentIndex == 0 
                            || !run[segmentIndex - 1].SegmentHistory.TryGetValue(nullSegment.Key, out segmentTime)
                            || segmentTime[method] != null)
                        {
                            var prediction = TrackBranch(run, currentTime, segmentIndex + 1, nullSegment.Key, method);
                            PopulatePrediction(predictions, prediction.Time[method], prediction.Index);
                        }
                    }
                }
                if (useCurrentRun)
                {
                    var currentRunPrediction = TrackCurrentRun(run, currentTime, segmentIndex, method);
                    PopulatePrediction(predictions, currentRunPrediction.Time[method], currentRunPrediction.Index);
                }
                var personalBestRunPrediction = TrackPersonalBestRun(run, currentTime, segmentIndex, method);
                PopulatePrediction(predictions, personalBestRunPrediction.Time[method], personalBestRunPrediction.Index);
            }
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, int startIndex, int endIndex, IList<TimeSpan?> predictions, bool simpleCalculation = false, bool useCurrentRun = true, TimingMethod method = TimingMethod.RealTime)
        {
            int segmentIndex = 0;
            TimeSpan? currentTime = TimeSpan.Zero;
            predictions[startIndex] = TimeSpan.Zero;
            foreach (var segment in run.Skip(startIndex).Take(endIndex - startIndex + 1))
            {
                currentTime = predictions[segmentIndex];
                PopulatePredictions(run, currentTime, segmentIndex, predictions, simpleCalculation, useCurrentRun, method);
                segmentIndex++;
            }
            return predictions[endIndex + 1];
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, bool simpleCalculation = false, bool useCurrentRun = true, TimingMethod method = TimingMethod.RealTime)
        {
            var predictions = new TimeSpan?[run.Count + 1];
            return CalculateSumOfBest(run, 0, run.Count() - 1, predictions, simpleCalculation, useCurrentRun, method);
        }

        public static void Clean(IRun run, TimingMethod method, CleanUpCallback callback = null)
        {
            var predictions = new TimeSpan?[run.Count + 1];
            CalculateSumOfBest(run, 0, run.Count() - 1, predictions, true, false, method);
            int segmentIndex = 0;
            TimeSpan? currentTime = TimeSpan.Zero;
            foreach (var segment in run)
            {
                currentTime = predictions[segmentIndex];
                foreach (var nullSegment in run[segmentIndex].SegmentHistory.Where(x => !x.Value[method].HasValue))
                {
                    var prediction = TrackBranch(run, currentTime, segmentIndex + 1, nullSegment.Key, method);
                    CheckPrediction(run, predictions, prediction.Time[method], segmentIndex - 1, prediction.Index - 1, nullSegment.Key, method, callback);
                } 
                segmentIndex++;
            }
        }

        private static void CheckPrediction(IRun run, TimeSpan?[] predictions, TimeSpan? predictedTime, int startingIndex, int endingIndex, int runIndex, TimingMethod method, CleanUpCallback callback)
        {
            if (predictedTime.HasValue && (!predictions[endingIndex + 1].HasValue || predictedTime < predictions[endingIndex + 1].Value))
            {
                Time segmentHistoryElement;
                if (run[endingIndex].SegmentHistory.TryGetValue(runIndex, out segmentHistoryElement))
                {
                    var parameters = new CleanUpCallbackParameters
                    {
                        startingSegment = startingIndex >= 0 ? run[startingIndex] : null,
                        endingSegment = endingIndex >= 0 ? run[endingIndex] : null,
                        timeBetween = segmentHistoryElement[method].Value,
                        combinedSumOfBest = predictions[endingIndex + 1].HasValue ? (TimeSpan?)(predictions[endingIndex + 1].Value - predictions[startingIndex + 1].Value) : null,
                        attempt = run.AttemptHistory.FirstOrDefault(x => x.Index == runIndex),
                        method = method
                    };
                    if (callback == null || callback(parameters))
                    {
                        run[endingIndex].SegmentHistory.Remove(runIndex);
                    }
                }
            }
        }

        public static void Clean(IRun run, CleanUpCallback callback = null)
        {
            Clean(run, TimingMethod.RealTime, callback);
            Clean(run, TimingMethod.GameTime, callback);
        }

        public class CleanUpCallbackParameters
        {
            public ISegment startingSegment;
            public ISegment endingSegment;
            public TimeSpan timeBetween;
            public TimeSpan? combinedSumOfBest;
            public Attempt attempt;
            public TimingMethod method;
        }

        public delegate bool CleanUpCallback(CleanUpCallbackParameters parameters);
    }
}
