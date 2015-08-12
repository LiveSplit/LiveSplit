using System;
using System.Collections.Generic;
using System.Linq;

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

        private static void PopulatePredictions(IRun run, TimeSpan? currentTime, int segmentIndex, IList<TimeSpan?> predictions, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            if (currentTime != null)
            {
                PopulatePrediction(predictions, currentTime + run[segmentIndex].BestSegmentTime[method], segmentIndex + 1);
                if (!simpleCalculation)
                {
                    foreach (var nullSegment in run[segmentIndex].SegmentHistory.Where(x => !x.Time[method].HasValue))
                    {
                        var segmentTime = segmentIndex > 0 ? run[segmentIndex - 1].SegmentHistory.FirstOrDefault(x => x.Index == nullSegment.Index) : null;
                        if (segmentTime == null || segmentTime.Time[method] != null)
                        {
                            var prediction = SumOfSegmentsHelper.TrackBranch(run, currentTime, segmentIndex + 1, nullSegment.Index, method);
                            PopulatePrediction(predictions, prediction.Time[method], prediction.Index);
                        }
                    }
                }
                var currentRunPrediction = SumOfSegmentsHelper.TrackCurrentRun(run, currentTime, segmentIndex, method);
                PopulatePrediction(predictions, currentRunPrediction.Time[method], currentRunPrediction.Index);
                var personalBestRunPrediction = SumOfSegmentsHelper.TrackPersonalBestRun(run, currentTime, segmentIndex, method);
                PopulatePrediction(predictions, personalBestRunPrediction.Time[method], personalBestRunPrediction.Index);
            }
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, int startIndex, int endIndex, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            var predictions = new TimeSpan?[run.Count + 1];
            return CalculateSumOfBest(run, startIndex, endIndex, predictions, simpleCalculation, method);
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, int startIndex, int endIndex, IList<TimeSpan?> predictions, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            int segmentIndex = 0;
            TimeSpan? currentTime = TimeSpan.Zero;
            predictions[startIndex] = TimeSpan.Zero;
            foreach (var segment in run.Skip(startIndex).Take(endIndex - startIndex + 1))
            {
                currentTime = predictions[segmentIndex];
                PopulatePredictions(run, currentTime, segmentIndex, predictions, simpleCalculation, method);
                segmentIndex++;
            }
            return predictions[endIndex + 1];
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, int endIndex, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            return CalculateSumOfBest(run, 0, endIndex, simpleCalculation, method);
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, int startIndex, TimeSpan? startTime, int endIndex, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            return CalculateSumOfBest(run, startIndex, endIndex, simpleCalculation, method) + startTime;
        }

        public static TimeSpan? CalculateSumOfBest(IRun run, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            return CalculateSumOfBest(run, run.Count() - 1, simpleCalculation, method);
        }

        public static void Clean(IRun run, TimingMethod method, CleanUpCallback callback = null)
        {
            var predictions = new TimeSpan?[run.Count + 1];
            CalculateSumOfBest(run, 0, run.Count() - 1, predictions, true, method);
            int segmentIndex = 0;
            TimeSpan? currentTime = TimeSpan.Zero;
            foreach (var segment in run)
            {
                currentTime = predictions[segmentIndex];
                foreach (var nullSegment in run[segmentIndex].SegmentHistory.Where(x => !x.Time[method].HasValue))
                {
                    var prediction = SumOfSegmentsHelper.TrackBranch(run, currentTime, segmentIndex + 1, nullSegment.Index, method);
                    CheckPrediction(run, predictions, prediction.Time[method], segmentIndex - 1, prediction.Index - 1, nullSegment.Index, method, callback);
                } 
                segmentIndex++;
            }
        }

        private static void CheckPrediction(IRun run, TimeSpan?[] predictions, TimeSpan? predictedTime, int startingIndex, int endingIndex, int runIndex, TimingMethod method = TimingMethod.RealTime, CleanUpCallback callback = null)
        {
            if (predictedTime.HasValue && (!predictions[endingIndex + 1].HasValue || predictedTime < predictions[endingIndex + 1].Value))
            {
                var segmentHistoryElement = run[endingIndex].SegmentHistory.FirstOrDefault(x => x.Index == runIndex);
                var parameters = new CleanUpCallbackParameters
                {
                    startingSegment = startingIndex >= 0 ? run[startingIndex] : null,
                    endingSegment = endingIndex >= 0 ? run[endingIndex] : null,
                    timeBetween = segmentHistoryElement.Time[method].Value,
                    combinedSumOfBest = predictions[endingIndex + 1].HasValue ? (TimeSpan?)(predictions[endingIndex + 1].Value - predictions[startingIndex + 1].Value) : null,
                    attempt = run.AttemptHistory.FirstOrDefault(x => x.Index == runIndex),
                    method = method
                };
                if (callback == null || callback(parameters))
                {
                    run[endingIndex].SegmentHistory.Remove(segmentHistoryElement);
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
