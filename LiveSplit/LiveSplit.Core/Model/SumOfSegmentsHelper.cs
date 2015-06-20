using System;
using System.Collections.Generic;
using System.Linq;

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
            var segmentTime = segmentIndex > 1 ? run[segmentIndex - 2].SegmentHistory.FirstOrDefault(x => x.Index == runIndex) : null;
            if (segmentTime == null || segmentTime.Time[method] != null)
            {
                while (segmentIndex < run.Count)
                {
                    segmentTime = run[segmentIndex].SegmentHistory.FirstOrDefault(x => x.Index == runIndex);
                    if (segmentTime != null)
                    {
                        var curTime = segmentTime.Time[method];
                        if (curTime.HasValue)
                        {
                            return new IndexedTime(new Time(method, curTime + currentTime), segmentIndex + 1);
                        }
                    }
                    else break;
                    segmentIndex++;
                }
            }
            return new IndexedTime(default(Time), 0);
        }
    }
}
