using System;
using System.Collections.Generic;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.Model
{
    public class TimeMust
    {
        private const long AnyTickValue = 7776000000000;
        private const string TimeSerializationAsString = "9.00:00:00 | 03:00:00";
        private static readonly TimeSpan AnyTimeSpan = TimeSpan.FromTicks(AnyTickValue);
        private static readonly TimeSpan AnotherTimeSpan = TimeSpan.FromHours(3);

        [Fact]
        public void ConstructZeroCorrectly()
        {
            var zeroTimeSpan = TimeSpan.FromTicks(0);
            Assert.Equal(zeroTimeSpan, Time.Zero.RealTime);
            Assert.Equal(zeroTimeSpan, Time.Zero.GameTime);
        }

        [Theory]
        [MemberData(nameof(TimeConstructorFeeder))]
        public void BeInitializedCorrectly(TimeSpan? anyRealTime, TimeSpan? anyGameTime)
        {
            var sut = new Time(anyRealTime, anyGameTime);
            Assert.Equal(anyRealTime, sut.RealTime);
            Assert.Equal(anyRealTime, sut[TimingMethod.RealTime]);
            Assert.Equal(anyGameTime, sut.GameTime);
            Assert.Equal(anyGameTime, sut[TimingMethod.GameTime]);
        }

        public static IEnumerable<object[]> TimeConstructorFeeder()
        {
            object[] possibleValues = { null, TimeSpan.Zero };
            foreach (var anyRealTime in possibleValues)
            {
                foreach (var anyGameTime in possibleValues)
                {
                    yield return new object[] { anyRealTime, anyGameTime };
                }
            }
        }

        [Fact]
        public void BeInitializedWithAnotherTimeCorrectly()
        {
            var anyTime = new Time(AnyTimeSpan, TimeSpan.Zero);
            var sut = new Time(anyTime);
            Assert.Equal(anyTime.RealTime, sut.RealTime);
            Assert.Equal(anyTime.GameTime, sut.GameTime);
        }

        [Fact]
        public void InitializeGameTimeCorrectly_WhenUsingGameTimimgMethod()
        {
            var sut = new Time(TimingMethod.GameTime, AnyTimeSpan);
            Assert.Null(sut.RealTime);
            Assert.Equal(AnyTimeSpan, sut.GameTime);
        }

        [Fact]
        public void InitializeRealTimeCorrectly_WhenUsingRealTimimgMethod()
        {
            var sut = new Time(TimingMethod.RealTime, AnyTimeSpan);
            Assert.Equal(AnyTimeSpan, sut.RealTime);
            Assert.Null(sut.GameTime);
        }

        [Theory]
        [MemberData(nameof(StringSerializationFeeder))]
        public void SerializeToStringCorrectly(TimeSpan? realTime, TimeSpan? gameTime, string expectedText)
        {
            var sut = new Time(realTime, gameTime);
            Assert.Equal(expectedText, sut.ToString());
        }

        public static IEnumerable<object[]> StringSerializationFeeder()
        {
            yield return new object[] { AnyTimeSpan, AnotherTimeSpan, "9.00:00:00 | 03:00:00" };
            yield return new object[] { null, AnotherTimeSpan, " | 03:00:00" };
            yield return new object[] { AnyTimeSpan, null, "9.00:00:00 | " };
            yield return new object[] { null, null, " | " };
        }

        [Theory]
        [MemberData(nameof(StringDeserializationFeeder))]
        public void DeserializeFromStringCorrectly(string text, TimeSpan? expectedRealTime, TimeSpan? expectedGameTime)
        {
            var sut = Time.ParseText(text);
            Assert.Equal(expectedRealTime, sut.RealTime);
            Assert.Equal(expectedGameTime, sut.GameTime);
        }

        public static IEnumerable<object[]> StringDeserializationFeeder()
        {
            yield return new object[] { "9.00:00:00 | 03:00:00", AnyTimeSpan, AnotherTimeSpan };
            yield return new object[] { "| 03:00:00", null, AnotherTimeSpan };
            yield return new object[] { "9.00:00:00 | ", AnyTimeSpan, null };
            yield return new object[] { "|", null, null };
            yield return new object[] { string.Empty, null, null };
            yield return new object[] { "invalid|4:1.0", null, null };
        }
    }
}
