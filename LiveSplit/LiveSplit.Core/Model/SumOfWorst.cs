using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.Model
{
    public static class SumOfWorst
    {
        private static void PopulatePrediction(IList<TimeSpan?> predictions, TimeSpan? predictedTime, int segmentIndex)
        {
            if (predictedTime.HasValue && (!predictions[segmentIndex].HasValue || predictedTime > predictions[segmentIndex].Value))
            {
                predictions[segmentIndex] = predictedTime;
            }
        }

        private static void PopulatePredictions(IRun run, TimeSpan? currentTime, int segmentIndex, IList<TimeSpan?> predictions, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
        {
            if (currentTime != null)
            {
                PopulatePrediction(predictions, currentTime + run[segmentIndex].BestSegmentTime[method], segmentIndex + 1);
                foreach (var segment in run[segmentIndex].SegmentHistory)
                {
                    var segmentTime = segmentIndex > 0 ? run[segmentIndex - 1].SegmentHistory.FirstOrDefault(x => x.Index == segment.Index) : null;
                    if (segmentTime == null || segmentTime.Time[method] != null)
                    {
                        var prediction = SumOfSegmentsHelper.TrackBranch(run, currentTime, segmentIndex, segment.Index, method);
                        PopulatePrediction(predictions, prediction.Time[method], prediction.Index);
                    }
                }
                var currentRunPrediction = SumOfSegmentsHelper.TrackCurrentRun(run, currentTime, segmentIndex, method);
                PopulatePrediction(predictions, currentRunPrediction.Time[method], currentRunPrediction.Index);
                var personalBestRunPrediction = SumOfSegmentsHelper.TrackPersonalBestRun(run, currentTime, segmentIndex, method);
                PopulatePrediction(predictions, personalBestRunPrediction.Time[method], personalBestRunPrediction.Index);
            }
        }

        

        public static TimeSpan? CalculateSumOfWorst(IRun run, int startIndex, int endIndex, IList<TimeSpan?> predictions, bool simpleCalculation, TimingMethod method = TimingMethod.RealTime)
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
    }
}
