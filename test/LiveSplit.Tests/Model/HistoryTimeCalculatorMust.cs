using System;
using System.Collections.Generic;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

using Xunit;

namespace LiveSplit.Tests.Model;

public class HistoryTimeCalculatorMust
{
    private static readonly TimeSpan _seg1 = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan _seg2 = TimeSpan.FromSeconds(45);
    private static readonly TimeSpan _seg3 = TimeSpan.FromSeconds(60);

    private static IRun CreateThreeSegmentRun()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1"),
            new Segment("S2"),
            new Segment("S3")
        };
        // PB split times: 30s, 1:15, 2:15
        run[0].PersonalBestSplitTime = new Time(TimingMethod.RealTime, _seg1);
        run[1].PersonalBestSplitTime = new Time(TimingMethod.RealTime, _seg1 + _seg2);
        run[2].PersonalBestSplitTime = new Time(TimingMethod.RealTime, _seg1 + _seg2 + _seg3);
        // Attempt #1: seg times = 30, 45, 60
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg1);
        run[1].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg2);
        run[2].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg3);
        return run;
    }

    [Fact]
    public void CalculateSplitTimesCorrectly()
    {
        IRun run = CreateThreeSegmentRun();
        IList<TimeSpan?> splitTimes = HistoryTimeCalculator.GetSplitTimesForAttempt(run, 1, TimingMethod.RealTime);

        Assert.Equal(3, splitTimes.Count);
        Assert.Equal(_seg1, splitTimes[0]);
        Assert.Equal(_seg1 + _seg2, splitTimes[1]);
        Assert.Equal(_seg1 + _seg2 + _seg3, splitTimes[2]);
    }

    [Fact]
    public void CalculateSegmentTimesCorrectly()
    {
        IRun run = CreateThreeSegmentRun();
        IList<TimeSpan?> segTimes = HistoryTimeCalculator.GetSegmentTimesForAttempt(run, 1, TimingMethod.RealTime);

        Assert.Equal(3, segTimes.Count);
        Assert.Equal(_seg1, segTimes[0]);
        Assert.Equal(_seg2, segTimes[1]);
        Assert.Equal(_seg3, segTimes[2]);
    }

    [Fact]
    public void HandleSparseData()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1"),
            new Segment("S2"),
            new Segment("S3")
        };
        // Attempt #2 only has first 2 segments (reset at segment 3)
        run[0].SegmentHistory[2] = new Time(TimingMethod.RealTime, _seg1);
        run[1].SegmentHistory[2] = new Time(TimingMethod.RealTime, _seg2);
        // Segment 3 has no entry for attempt 2

        IList<TimeSpan?> segTimes = HistoryTimeCalculator.GetSegmentTimesForAttempt(run, 2, TimingMethod.RealTime);
        IList<TimeSpan?> splitTimes = HistoryTimeCalculator.GetSplitTimesForAttempt(run, 2, TimingMethod.RealTime);

        Assert.Equal(_seg1, segTimes[0]);
        Assert.Equal(_seg2, segTimes[1]);
        Assert.Null(segTimes[2]);

        Assert.Equal(_seg1, splitTimes[0]);
        Assert.Equal(_seg1 + _seg2, splitTimes[1]);
        Assert.Null(splitTimes[2]);
    }

    [Fact]
    public void CalculateSegmentBestDiffCorrectly()
    {
        // BestSegmentTime: 30, 45, 60
        // Attempt segment times: 28, 52, 60
        // Expected diffs: -2, +7, 0
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1"),
            new Segment("S2"),
            new Segment("S3")
        };
        run[0].BestSegmentTime = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(30));
        run[1].BestSegmentTime = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(45));
        run[2].BestSegmentTime = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(60));

        run[0].SegmentHistory[3] = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(28));
        run[1].SegmentHistory[3] = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(52));
        run[2].SegmentHistory[3] = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(60));

        IList<TimeSpan?> diffs = HistoryTimeCalculator.GetSegmentBestDiffsForAttempt(run, 3, TimingMethod.RealTime);

        Assert.Equal(3, diffs.Count);
        Assert.Equal(TimeSpan.FromSeconds(-2), diffs[0]); // 28 - 30 = -2s (faster)
        Assert.Equal(TimeSpan.FromSeconds(7), diffs[1]);  // 52 - 45 = +7s (slower)
        Assert.Equal(TimeSpan.Zero, diffs[2]);            // 60 - 60 = 0s (matched best)
    }

    [Fact]
    public void HandleNullBestSegmentTime()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1")
        };
        // No BestSegmentTime set (null)
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg1);

        IList<TimeSpan?> diffs = HistoryTimeCalculator.GetSegmentBestDiffsForAttempt(run, 1, TimingMethod.RealTime);

        Assert.Single(diffs);
        Assert.Null(diffs[0]); // diff is null when BestSegmentTime is null
    }

    [Fact]
    public void PropagateNullWhenMethodValueAbsent()
    {
        // Key exists in SegmentHistory but GameTime value is null (RealTime-only recording)
        // When viewing under GameTime: null must propagate — subsequent segments should also be null
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1"),
            new Segment("S2"),
            new Segment("S3")
        };

        // Attempt 1: all segments have RealTime but NOT GameTime
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg1); // GameTime = null
        run[1].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg2); // GameTime = null
        run[2].SegmentHistory[1] = new Time(TimingMethod.RealTime, _seg3); // GameTime = null

        IList<TimeSpan?> splitTimes = HistoryTimeCalculator.GetSplitTimesForAttempt(run, 1, TimingMethod.GameTime);

        // All split times must be null because GameTime is absent in each history entry
        Assert.Null(splitTimes[0]);
        Assert.Null(splitTimes[1]);
        Assert.Null(splitTimes[2]);
    }
}
