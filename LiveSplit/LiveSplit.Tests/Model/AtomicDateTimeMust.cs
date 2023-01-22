using System;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.Model
{
    public class AtomicDateTimeMust
    {
        private static readonly DateTime AnyDateTime = new DateTime(2020, 12, 13);
        private static readonly DateTime AnotherDateTime = new DateTime(2020, 12, 4);
        private const long DateTimeDifference = 7776000000000;

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
            var difference = minuend - subtrahend;
            Assert.Equal(DateTimeDifference, difference.Ticks);
        }

        [Fact]
        public void CalculateDifferenceWithDateTimeCorrectly()
        {
            var minuend = new AtomicDateTime(AnyDateTime, false);
            var subtrahend = AnotherDateTime;
            var difference = minuend - subtrahend;
            Assert.Equal(DateTimeDifference, difference.Ticks);
        }
    }
}
