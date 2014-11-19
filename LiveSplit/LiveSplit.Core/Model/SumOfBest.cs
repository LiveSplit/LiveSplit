using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.Model
{
    public static class SumOfBest
    {
        private static void PopulatePrediction(IList<TimeSpan?> predictions, TimeSpan? predictedTime, int segmentIndex, TimingMethod method = TimingMethod.RealTime)
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
                        var prediction = TrackBranch(run, currentTime, segmentIndex + 1, nullSegment.Index);
                        PopulatePrediction(predictions, prediction.Time[method], prediction.Index, method);
                    }
                }
                var currentRunPrediction = TrackCurrentRun(run, currentTime, segmentIndex);
                PopulatePrediction(predictions, currentRunPrediction.Time[method], currentRunPrediction.Index, method);
                var personalBestRunPrediction = TrackPersonalBestRun(run, currentTime, segmentIndex);
                PopulatePrediction(predictions, personalBestRunPrediction.Time[method], personalBestRunPrediction.Index, method);
            }
        }

        private static IndexedTime TrackCurrentRun(IRun run, TimeSpan? currentTime, int segmentIndex, TimingMethod method = TimingMethod.RealTime)
        {
            if (segmentIndex > 0 && !run[segmentIndex - 1].SplitTime[method].HasValue)
                return new IndexedTime(default(Time), 0);
            var firstSplitTime = segmentIndex < 1 ? TimeSpan.Zero : run[segmentIndex - 1].SplitTime[method];
            while (segmentIndex < run.Count)
            {
                var secondSplitTime = run[segmentIndex].SplitTime[method];
                if (secondSplitTime.HasValue)
                {
                    return new IndexedTime(new Time(secondSplitTime - firstSplitTime + currentTime), segmentIndex + 1);
                }
                segmentIndex++;
            }
            return new IndexedTime(default(Time), 0);
        }

        private static IndexedTime TrackPersonalBestRun(IRun run, TimeSpan? currentTime, int segmentIndex, TimingMethod method = TimingMethod.RealTime)
        {
            if (segmentIndex > 0 && !run[segmentIndex - 1].PersonalBestSplitTime[method].HasValue)
                return new IndexedTime(default(Time), 0);
            var firstSplitTime = segmentIndex < 1 ? TimeSpan.Zero : run[segmentIndex - 1].PersonalBestSplitTime[method];
            while (segmentIndex < run.Count)
            {
                var secondSplitTime = run[segmentIndex].PersonalBestSplitTime[method];
                if (secondSplitTime.HasValue)
                {
                    return new IndexedTime(new Time(secondSplitTime - firstSplitTime + currentTime), segmentIndex + 1);
                }
                segmentIndex++;
            }
            return new IndexedTime(default(Time), 0);
        }

        private static IndexedTime TrackBranch(IRun run, TimeSpan? currentTime, int segmentIndex, int runIndex, TimingMethod method = TimingMethod.RealTime)
        {
            while (segmentIndex < run.Count)
            {
                var segmentTime = run[segmentIndex].SegmentHistory.Where(x => x.Index == runIndex).FirstOrDefault();
                if (segmentTime != null)
                {
                    var curTime = segmentTime.Time[method];
                    /*if (run[segmentIndex].BestSegmentTime != null && curTime < run[segmentIndex].BestSegmentTime)
                        curTime = run[segmentIndex].BestSegmentTime;*/
                    if (curTime.HasValue)
                    {
                        return new IndexedTime(new Time(curTime + currentTime), segmentIndex + 1);
                    }
                }
                else break;
                segmentIndex++;
            }
            return new IndexedTime(default(Time), 0);
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
    }
}
