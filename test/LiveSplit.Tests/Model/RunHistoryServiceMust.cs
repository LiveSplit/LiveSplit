using System;
using System.Collections.Generic;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

using Xunit;

namespace LiveSplit.Tests.Model;

public class RunHistoryServiceMust
{
    private static readonly TimeSpan _anySegTime = TimeSpan.FromSeconds(30);

    private static IRun CreateRun(int segmentCount, int attemptCount, TimeSpan segmentTime)
    {
        var run = new Run(new StandardComparisonGeneratorsFactory());
        for (int s = 0; s < segmentCount; s++)
        {
            var segment = new Segment($"Segment {s + 1}");
            run.Add(segment);
        }

        TimeSpan cumulative = TimeSpan.Zero;
        for (int s = 0; s < segmentCount; s++)
        {
            cumulative += segmentTime;
            run[s].PersonalBestSplitTime = new Time(TimingMethod.RealTime, cumulative);
            run[s].BestSegmentTime = new Time(TimingMethod.RealTime, segmentTime);
        }

        for (int a = 1; a <= attemptCount; a++)
        {
            TimeSpan total = TimeSpan.Zero;
            for (int s = 0; s < segmentCount; s++)
            {
                run[s].SegmentHistory[a] = new Time(TimingMethod.RealTime, segmentTime);
                total += segmentTime;
            }

            run.AttemptHistory.Add(new Attempt(a, new Time(TimingMethod.RealTime, total), null, null, null));
        }

        return run;
    }

    [Fact]
    public void FilterCompletedRunsOnly()
    {
        IRun run = CreateRun(3, 0, _anySegTime);

        // Attempt 1: completed
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, _anySegTime);
        run[1].SegmentHistory[1] = new Time(TimingMethod.RealTime, _anySegTime);
        run[2].SegmentHistory[1] = new Time(TimingMethod.RealTime, _anySegTime);
        run.AttemptHistory.Add(new Attempt(1, new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(90)), null, null, null));

        // Attempt 2: not completed (no final time)
        run[0].SegmentHistory[2] = new Time(TimingMethod.RealTime, _anySegTime);
        run.AttemptHistory.Add(new Attempt(2, new Time(), null, null, null));

        // Attempt 3: completed
        run[0].SegmentHistory[3] = new Time(TimingMethod.RealTime, _anySegTime);
        run[1].SegmentHistory[3] = new Time(TimingMethod.RealTime, _anySegTime);
        run[2].SegmentHistory[3] = new Time(TimingMethod.RealTime, _anySegTime);
        run.AttemptHistory.Add(new Attempt(3, new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(90)), null, null, null));

        var filter = new RunHistoryFilter(CompletedOnly: true, Method: TimingMethod.RealTime);
        IList<Attempt> result = RunHistoryService.GetFilteredAttempts(run, filter);

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, a => a.Index == 2);
    }

    [Fact]
    public void ReturnAllWhenNoFilters()
    {
        IRun run = CreateRun(2, 5, _anySegTime);
        var filter = new RunHistoryFilter();
        IList<Attempt> result = RunHistoryService.GetFilteredAttempts(run, filter);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void ReturnAttemptsSortedByIndexDescending()
    {
        IRun run = CreateRun(2, 3, _anySegTime);
        var filter = new RunHistoryFilter();
        IList<Attempt> result = RunHistoryService.GetFilteredAttempts(run, filter);

        Assert.Equal(3, result[0].Index);
        Assert.Equal(2, result[1].Index);
        Assert.Equal(1, result[2].Index);
    }

    [Fact]
    public void PaginateCorrectly()
    {
        IRun run = CreateRun(1, 25, _anySegTime);
        var filter = new RunHistoryFilter();
        IList<Attempt> all = RunHistoryService.GetFilteredAttempts(run, filter);

        IList<Attempt> page1 = RunHistoryService.GetPage(all, 1, 10);
        IList<Attempt> page2 = RunHistoryService.GetPage(all, 2, 10);
        IList<Attempt> page3 = RunHistoryService.GetPage(all, 3, 10);

        Assert.Equal(10, page1.Count);
        Assert.Equal(10, page2.Count);
        Assert.Equal(5, page3.Count);
    }

    [Fact]
    public void HandleLastPageWithFewerItems()
    {
        IRun run = CreateRun(1, 7, _anySegTime);
        var filter = new RunHistoryFilter();
        IList<Attempt> all = RunHistoryService.GetFilteredAttempts(run, filter);

        IList<Attempt> page2 = RunHistoryService.GetPage(all, 2, 5);
        Assert.Equal(2, page2.Count);
    }

    [Fact]
    public void ReturnEmptyForOutOfRangePage()
    {
        IRun run = CreateRun(1, 5, _anySegTime);
        var filter = new RunHistoryFilter();
        IList<Attempt> all = RunHistoryService.GetFilteredAttempts(run, filter);

        IList<Attempt> result = RunHistoryService.GetPage(all, 99, 10);
        Assert.Empty(result);
    }

    [Fact]
    public void FilterPbSegmentsOnly()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory());
        var seg = new Segment("S1");
        run.Add(seg);

        var bestTime = TimeSpan.FromSeconds(25);
        run[0].BestSegmentTime = new Time(TimingMethod.RealTime, bestTime);

        // Attempt 1: segment time equals best segment (is a PB segment)
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, bestTime);
        run.AttemptHistory.Add(new Attempt(1, new Time(TimingMethod.RealTime, bestTime), null, null, null));

        // Attempt 2: segment time does not match
        run[0].SegmentHistory[2] = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(40));
        run.AttemptHistory.Add(new Attempt(2, new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(40)), null, null, null));

        var filter = new RunHistoryFilter(PbSegmentsOnly: true, Method: TimingMethod.RealTime);
        IList<Attempt> result = RunHistoryService.GetFilteredAttempts(run, filter);

        Assert.Single(result);
        Assert.Equal(1, result[0].Index);
    }

    [Fact]
    public void CombineFiltersWithAndLogic()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1")
        };
        var bestTime = TimeSpan.FromSeconds(25);
        run[0].BestSegmentTime = new Time(TimingMethod.RealTime, bestTime);

        // Attempt 1: completed AND has PB segment
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, bestTime);
        run.AttemptHistory.Add(new Attempt(1, new Time(TimingMethod.RealTime, bestTime), null, null, null));

        // Attempt 2: completed but NO PB segment
        run[0].SegmentHistory[2] = new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(40));
        run.AttemptHistory.Add(new Attempt(2, new Time(TimingMethod.RealTime, TimeSpan.FromSeconds(40)), null, null, null));

        // Attempt 3: has PB segment but NOT completed
        run[0].SegmentHistory[3] = new Time(TimingMethod.RealTime, bestTime);
        run.AttemptHistory.Add(new Attempt(3, new Time(), null, null, null));

        var filter = new RunHistoryFilter(CompletedOnly: true, PbSegmentsOnly: true, Method: TimingMethod.RealTime);
        IList<Attempt> result = RunHistoryService.GetFilteredAttempts(run, filter);

        Assert.Single(result);
        Assert.Equal(1, result[0].Index);
    }

    [Fact]
    public void FilterByDateRange()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1")
        };
        run[0].SegmentHistory[1] = new Time(TimingMethod.RealTime, _anySegTime);
        run[0].SegmentHistory[2] = new Time(TimingMethod.RealTime, _anySegTime);
        run[0].SegmentHistory[3] = new Time(TimingMethod.RealTime, _anySegTime);

        var jan2024 = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var feb2024 = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc);
        var mar2024 = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);

        run.AttemptHistory.Add(new Attempt(1, new Time(TimingMethod.RealTime, _anySegTime), new AtomicDateTime(jan2024, false), null, null));
        run.AttemptHistory.Add(new Attempt(2, new Time(TimingMethod.RealTime, _anySegTime), new AtomicDateTime(feb2024, false), null, null));
        run.AttemptHistory.Add(new Attempt(3, new Time(TimingMethod.RealTime, _anySegTime), new AtomicDateTime(mar2024, false), null, null));

        // Filter to only include Feb 2024
        var filter = new RunHistoryFilter(
            FromDate: new DateTime(2024, 2, 1),
            ToDate: new DateTime(2024, 2, 28, 23, 59, 59),
            Method: TimingMethod.RealTime
        );
        IList<Attempt> result = RunHistoryService.GetFilteredAttempts(run, filter);

        Assert.Single(result);
        Assert.Equal(2, result[0].Index);
    }
}
