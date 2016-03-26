using System;

namespace LiveSplit.Model
{
    public static class SumOfSegmentsHelper
    {
        public static IndexedTime TrackCurrentRun(IRun run, TimeSpan? currentTime, int segmentIndex, TimingMethod method = TimingMethod.RealTime)
        {
            if (segmentIndex > 0 && !run[segmentIndex - 1].SplitTime[method].HasValue)
                return new IndexedTime(default(Time), 0);
            var firstSplitTime = segmentIndex < 1 ? TimeSpan.Zero : run[segmentIndex - 1].SplitTime[method];
            while (segmentIndex < run.Count)
            {
                var secondSplitTime = run[segmentIndex].SplitTime[method];
                if (secondSplitTime.HasValue)
                {
                    return new IndexedTime(new Time(method, secondSplitTime - firstSplitTime + currentTime), segmentIndex + 1);
                }
                segmentIndex++;
            }
            return new IndexedTime(default(Time), 0);
        }

        public static IndexedTime TrackPersonalBestRun(IRun run, TimeSpan? currentTime, int segmentIndex, TimingMethod method = TimingMethod.RealTime)
        {
            if (segmentIndex > 0 && !run[segmentIndex - 1].PersonalBestSplitTime[method].HasValue)
                return new IndexedTime(default(Time), 0);
            var firstSplitTime = segmentIndex < 1 ? TimeSpan.Zero : run[segmentIndex - 1].PersonalBestSplitTime[method];
            while (segmentIndex < run.Count)
            {
                var secondSplitTime = run[segmentIndex].PersonalBestSplitTime[method];
                if (secondSplitTime.HasValue)
                {
                    return new IndexedTime(new Time(method, secondSplitTime - firstSplitTime + currentTime), segmentIndex + 1);
                }
                segmentIndex++;
            }
            return new IndexedTime(default(Time), 0);
        }

        public static IndexedTime TrackBranch(IRun run, TimeSpan? currentTime, int segmentIndex, int runIndex, TimingMethod method = TimingMethod.RealTime)
        {
            while (segmentIndex < run.Count)
            {
                Time segmentTime;
                if (run[segmentIndex].SegmentHistory.TryGetValue(runIndex, out segmentTime))
                {
                    var curTime = segmentTime[method];
                    if (curTime.HasValue)
                    {
                        return new IndexedTime(new Time(method, curTime + currentTime), segmentIndex + 1);
                    }
                }
                else break;
                segmentIndex++;
            }
            return new IndexedTime(default(Time), 0);
        }
    }
}
