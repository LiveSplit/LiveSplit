using System;
using System.Collections.Generic;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.Model
{
    public class AttemptMust
    {
        private static readonly DateTime AnyDateTime = new DateTime(2020, 12, 13);

        [Theory]
        [MemberData(nameof(AttemptConstructorFeeder))]
        public void BeInitializedCorrectly(AtomicDateTime? anyStart, AtomicDateTime? anyEnd, TimeSpan? anyPause)
        {
            var anyIndex = 0;
            var anyTime = Time.Zero;

            var sut = new Attempt(anyIndex, anyTime, anyStart, anyEnd, anyPause);
            Assert.Equal(anyIndex, sut.Index);
            Assert.Equal(anyTime, sut.Time);
            Assert.Equal(anyStart, sut.Started);
            Assert.Equal(anyEnd, sut.Ended);
            Assert.Equal(anyPause, sut.PauseTime);
        }

        public static IEnumerable<object[]> AttemptConstructorFeeder()
        {
            object[] possibleValues = { null, new AtomicDateTime(AnyDateTime, false) };
            foreach (var anyStart in possibleValues)
            {
                foreach (var anyEnd in possibleValues)
                {
                    foreach (var anyPause in new object[] { null, TimeSpan.Zero })
                    {
                        yield return new object[] { anyStart, anyEnd, anyPause };
                    }
                }
            }
        }
    }
}
