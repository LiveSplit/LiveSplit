using Xunit;
using LiveSplit.Model;
using static LiveSplit.Tests.Model.Constants;
using System;

namespace LiveSplit.Tests.Model
{
    public class SegmentHistoryMust
    {
        [Fact]
        public void CalculateMinimumIndexCorrectly_WhenHistoryIsEmpty()
        {
            var sut = new SegmentHistory();
            Assert.Equal(1, sut.GetMinIndex());
        }

        [Fact]
        public void CalculateMinimunIndexCorrectly_WhenIndexesAreAboveZero()
        {
            var sut = new SegmentHistory
            {
                { 7, new Time(AnyTimeSpan, YetAnotherTimeSpan) },
                { 3, new Time(YetAnotherTimeSpan, AnyTimeSpan) }
            };
            Assert.Equal(1, sut.GetMinIndex());
        }

        [Fact]
        public void CalculateMinimunIndexCorrectly_WhenIndexIsZero()
        {
            var sut = new SegmentHistory
            {
                { 7, new Time(AnyTimeSpan, YetAnotherTimeSpan) },
                { 0, new Time(YetAnotherTimeSpan, AnyTimeSpan) }
            };
            Assert.Equal(0, sut.GetMinIndex());
        }

        [Fact]
        public void BeInitializedCorrectly_WhenInitializingWithSegmentHistory()
        {
            var original = new SegmentHistory
            {
                { 7, new Time(AnyTimeSpan, YetAnotherTimeSpan) },
                { 0, new Time(YetAnotherTimeSpan, AnyTimeSpan) }
            };

            var sut = new SegmentHistory(original);
            Assert.NotSame(original, sut);
            Assert.Equal(original.Count, sut.Count);
            Assert.Equal(original[0], sut[0]);
            Assert.Equal(original[7], sut[7]);
        }

        [Fact]
        public void BeClonedCorrectly()
        {
            var original = new SegmentHistory
            {
                { 7, new Time(AnyTimeSpan, YetAnotherTimeSpan) },
                { 0, new Time(YetAnotherTimeSpan, AnyTimeSpan) }
            };

            var sut = original.Clone();
            Assert.NotSame(original, sut);
            Assert.Equal(original.Count, sut.Count);
            Assert.Equal(original[0], sut[0]);
            Assert.Equal(original[7], sut[7]);
        }

        [Fact]
        public void BeClonedCorrectly_WhenUsingIClonable()
        {
            var original = new SegmentHistory
            {
                { 7, new Time(AnyTimeSpan, YetAnotherTimeSpan) },
                { 0, new Time(YetAnotherTimeSpan, AnyTimeSpan) }
            };

            ICloneable cloneable = original;
            var sut = (SegmentHistory)cloneable.Clone();
            Assert.NotSame(original, sut);
            Assert.Equal(original.Count, sut.Count);
            Assert.Equal(original[0], sut[0]);
            Assert.Equal(original[7], sut[7]);
        }
    }
}
