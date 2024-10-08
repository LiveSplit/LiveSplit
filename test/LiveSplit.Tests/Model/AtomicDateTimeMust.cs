﻿using LiveSplit.Model;

using Xunit;

using static LiveSplit.Tests.Model.Constants;

namespace LiveSplit.Tests.Model;

public class AtomicDateTimeMust
{
    [Fact]
    public void ReturnTrue_WhenAtomicDateSyncedWithAtomicClock()
    {
        var sut = new AtomicDateTime(AnyDateTime, true);
        Assert.True(sut.SyncedWithAtomicClock);
        Assert.Equal(AnyDateTime, sut.Time);
    }

    [Fact]
    public void ReturnFalse_WhenAtomicDateNotSyncedWithAtomicClock()
    {
        var sut = new AtomicDateTime(AnyDateTime, false);
        Assert.False(sut.SyncedWithAtomicClock);
        Assert.Equal(AnyDateTime, sut.Time);
    }

    [Fact]
    public void CalculateDifferenceWithAtomicDateCorrectly()
    {
        var minuend = new AtomicDateTime(AnyDateTime, true);
        var subtrahend = new AtomicDateTime(AnotherDateTime, true);
        System.TimeSpan difference = minuend - subtrahend;
        Assert.Equal(DateTimeDifference, difference.Ticks);
    }

    [Fact]
    public void CalculateDifferenceWithDateTimeCorrectly()
    {
        var minuend = new AtomicDateTime(AnyDateTime, false);
        System.DateTime subtrahend = AnotherDateTime;
        System.TimeSpan difference = minuend - subtrahend;
        Assert.Equal(DateTimeDifference, difference.Ticks);
    }
}
