using System;
using Xunit;
using LiveSplit.Model;
using static LiveSplit.Tests.Model.Constants;

namespace LiveSplit.Tests.Model
{
    public class IndexedTimeMust
    {
        [Fact]
        public void BeInitializedCorrectly_WhenUsingDefaultConstructor()
        {
            var sut = new IndexedTime();
            Assert.Equal(default, sut.Time);
            Assert.Equal(default, sut.Index);
        }

        [Fact]
        public void BeInitializedCorrectly_WhenAssigningValuesInConstructor()
        {
            var anyTime = new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(AnyTickValue));
            var anyIndex = 1;
            var sut = new IndexedTime(anyTime, anyIndex);
            Assert.Equal(anyIndex, sut.Index);
            Assert.Equal(anyTime, sut.Time);
        }
    }
}
