using System;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

using Xunit;

namespace LiveSplit.Tests.Model;

public class AttemptDeletionHelperMust
{
    private static readonly TimeSpan _anyTime = TimeSpan.FromSeconds(30);

    private static IRun CreateRunWith5Attempts()
    {
        var run = new Run(new StandardComparisonGeneratorsFactory())
        {
            new Segment("S1"),
            new Segment("S2")
        };

        for (int a = 1; a <= 5; a++)
        {
            run[0].SegmentHistory[a] = new Time(TimingMethod.RealTime, _anyTime);
            run[1].SegmentHistory[a] = new Time(TimingMethod.RealTime, _anyTime);
            run.AttemptHistory.Add(new Attempt(a, new Time(TimingMethod.RealTime, _anyTime + _anyTime), null, null, null));
        }

        return run;
    }

    [Fact]
    public void RemoveAttemptFromHistory()
    {
        IRun run = CreateRunWith5Attempts();
        AttemptDeletionHelper.DeleteAttempt(run, 3);

        Assert.Equal(4, run.AttemptHistory.Count);
        Assert.DoesNotContain(run.AttemptHistory, a => a.Index == 3);
    }

    [Fact]
    public void RemoveFromAllSegmentHistories()
    {
        IRun run = CreateRunWith5Attempts();
        AttemptDeletionHelper.DeleteAttempt(run, 3);

        Assert.False(run[0].SegmentHistory.ContainsKey(3));
        Assert.False(run[1].SegmentHistory.ContainsKey(3));
    }

    [Fact]
    public void NotAffectOtherAttempts()
    {
        IRun run = CreateRunWith5Attempts();
        AttemptDeletionHelper.DeleteAttempt(run, 3);

        Assert.True(run[0].SegmentHistory.ContainsKey(1));
        Assert.True(run[0].SegmentHistory.ContainsKey(2));
        Assert.True(run[0].SegmentHistory.ContainsKey(4));
        Assert.True(run[0].SegmentHistory.ContainsKey(5));
        Assert.Equal(4, run.AttemptHistory.Count);
    }

    [Fact]
    public void HandleNonExistentAttempt()
    {
        IRun run = CreateRunWith5Attempts();
        // Should not throw
        AttemptDeletionHelper.DeleteAttempt(run, 999);

        // Data unchanged
        Assert.Equal(5, run.AttemptHistory.Count);
    }
}
