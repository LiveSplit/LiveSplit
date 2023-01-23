using System;
using System.Collections.Generic;
using System.Xml;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.Model
{
    public class AttemptMust
    {
        private static readonly DateTime AnyDateTime = new DateTime(2020, 12, 13);
        private static readonly DateTime AnotherDateTime = new DateTime(2020, 12, 4);
        private const long AnyTickValue = 7776000000000;
        private static readonly TimeSpan AnotherTimeSpan = TimeSpan.FromHours(3);
        private static readonly TimeSpan AnyTimeSpan = TimeSpan.FromTicks(AnyTickValue);
        private const long Difference = 7668000000000;

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

        [Theory]
        [MemberData(nameof(DurationFeeder))]
        public void CalculateDurationCorrectly(AtomicDateTime? anyStart, AtomicDateTime? anyEnd, TimeSpan? expectedResult)
        {
            var anyIndex = 1;
            var anyPause = TimeSpan.Zero;
            var sut = new Attempt(anyIndex, new Time(AnyTimeSpan, AnotherTimeSpan), anyStart, anyEnd, anyPause);
            Assert.Equal(expectedResult, sut.Duration);
        }

        public static IEnumerable<object[]> DurationFeeder()
        {
            yield return new object[] { null, null, AnyTimeSpan };
            yield return new object[] { null, new AtomicDateTime(AnyDateTime, true), AnyTimeSpan };
            yield return new object[] { new AtomicDateTime(AnyDateTime, true), null, AnyTimeSpan };
            yield return new object[] { new AtomicDateTime(AnotherDateTime, true), new AtomicDateTime(AnyDateTime, true), TimeSpan.FromTicks(AnyTickValue) };
            yield return new object[] { new AtomicDateTime(AnyDateTime, true), new AtomicDateTime(AnotherDateTime, true), TimeSpan.FromTicks(-AnyTickValue) };
            yield return new object[] { new AtomicDateTime(), new AtomicDateTime(), TimeSpan.Zero };
        }

        [Theory]
        [MemberData(nameof(JsonSerializationFeeder))]
        public void SerializeToJsonCorrectly(int anyIndex, Time anyTime, AtomicDateTime? anyStart, AtomicDateTime? anyEnd, TimeSpan? anyPause,
            string expectedId, string expectedRealTime, string expectedGameTime, string expectedStart, string expectedEnd, string expectedPause)
        {
            var sut = new Attempt(anyIndex, anyTime, anyStart, anyEnd, anyPause);

            var jsonText = sut.ToJson().ToString();
            Assert.Matches(expectedId, jsonText);
            Assert.Matches(expectedRealTime, jsonText);
            Assert.Matches(expectedGameTime, jsonText);
            Assert.Matches(expectedStart, jsonText);
            Assert.Matches(expectedEnd, jsonText);
            Assert.Matches(expectedPause, jsonText);
        }

        // FIXME: AtomicDateTime is not being serialized correctly (#2307)
        public static IEnumerable<object[]> JsonSerializationFeeder()
        {
            yield return new object[] { 0, Time.Zero, null, null, null, @"""id""\s*:\s*0", @"""realTime""\s*:\s*""00:00:00""", @"""gameTime""\s*:\s*""00:00:00""", @"""started""\s*:\s*null", @"""ended""\s*:\s*null", @"""pauseTime""\s*:\s*null" };
            yield return new object[] { 1, new Time(TimeSpan.FromTicks(AnyTickValue)), new AtomicDateTime(AnyDateTime, true), new AtomicDateTime(), TimeSpan.Zero, @"""id""\s*:\s*1", @"""realTime""\s*:\s*""9.00:00:00""", @"""gameTime""\s*:\s*null", @"""started""\s*:\s*""LiveSplit.Model.AtomicDateTime""", @"""ended""\s*:\s*""LiveSplit.Model.AtomicDateTime""", @"""pauseTime""\s*:\s*""00:00:00""" };
            yield return new object[] { 2, new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(AnyTickValue)), new AtomicDateTime(AnyDateTime, false), new AtomicDateTime(AnyDateTime, true), AnotherTimeSpan, @"""id""\s*:\s*2", @"""realTime""\s*:\s*""9.00:00:00""", @"""gameTime""\s*:\s*""9.00:00:00""", @"""started""\s*:\s*""LiveSplit.Model.AtomicDateTime""", @"""ended""\s*:\s*""LiveSplit.Model.AtomicDateTime""", @"""pauseTime""\s*:\s*""03:00:00""" };
        }

        [Fact]
        public void SerializeToXmlCorrectly_WhenAttemptIsEmpty()
        {
            var document = new XmlDocument();
            var attempt = new Attempt(0, Time.Zero, null, null, null);

            var sut = attempt.ToXml(document);
            var xmlText = sut.OuterXml;

            Assert.Equal("0", sut.Attributes["id"].Value);
            Assert.Null(sut.Attributes["started"]);
            Assert.Null(sut.Attributes["ended"]);
            Assert.Null(sut.Attributes["isStartedSynced"]);
            Assert.Null(sut.Attributes["isEndedSynced"]);
            Assert.Contains("<RealTime>00:00:00</RealTime>", xmlText);
            Assert.Contains("<GameTime>00:00:00</GameTime>", xmlText);
            Assert.DoesNotContain("<PauseTime>", xmlText);
        }

        [Fact]
        public void SerializeToXmlCorrectly_WhenAttemptHasStartedTime()
        {
            var document = new XmlDocument();
            var attempt = new Attempt(1, new Time(TimeSpan.FromTicks(AnyTickValue)), new AtomicDateTime(AnyDateTime, true), null, null);

            var sut = attempt.ToXml(document);
            var xmlText = sut.OuterXml;

            Assert.Equal("1", sut.Attributes["id"].Value);
            Assert.Equal("12/13/2020 03:00:00", sut.Attributes["started"].Value);
            Assert.Null(sut.Attributes["ended"]);
            Assert.Equal("True", sut.Attributes["isStartedSynced"].Value);
            Assert.Null(sut.Attributes["isEndedSynced"]);
            Assert.Contains("<RealTime>9.00:00:00</RealTime>", xmlText);
            Assert.DoesNotContain("<PauseTime>", xmlText);
            Assert.DoesNotContain("<GameTime>", xmlText);
        }

        [Fact]
        public void SerializeToXmlCorrectly_WhenAttemptHasEndedTime()
        {
            var document = new XmlDocument();
            var attempt = new Attempt(2, new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(AnyTickValue)), null, new AtomicDateTime(), null);

            var sut = attempt.ToXml(document);
            var xmlText = sut.OuterXml;

            Assert.Equal("2", sut.Attributes["id"].Value);
            Assert.Null(sut.Attributes["started"]);
            Assert.Equal("01/01/0001 03:00:00", sut.Attributes["ended"].Value);
            Assert.Null(sut.Attributes["isStartedSynced"]);
            Assert.Equal("False", sut.Attributes["isEndedSynced"].Value);
            Assert.Contains("<RealTime>9.00:00:00</RealTime>", xmlText);
            Assert.Contains("<GameTime>9.00:00:00</GameTime>", xmlText);
            Assert.DoesNotContain("<PauseTime>", xmlText);
        }

        [Fact]
        public void SerializeToXmlCorrectly_WhenAttemptHasPauseTime()
        {
            var document = new XmlDocument();
            var attempt = new Attempt(3, new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(AnyTickValue)), new AtomicDateTime(AnyDateTime, true), new AtomicDateTime(), AnotherTimeSpan);

            var sut = attempt.ToXml(document);
            var xmlText = sut.OuterXml;

            Assert.Equal("3", sut.Attributes["id"].Value);
            Assert.Equal("12/13/2020 03:00:00", sut.Attributes["started"].Value);
            Assert.Equal("01/01/0001 03:00:00", sut.Attributes["ended"].Value);
            Assert.Equal("True", sut.Attributes["isStartedSynced"].Value);
            Assert.Equal("False", sut.Attributes["isEndedSynced"].Value);
            Assert.Contains("<RealTime>9.00:00:00</RealTime>", xmlText);
            Assert.Contains("<GameTime>9.00:00:00</GameTime>", xmlText);
            Assert.Contains("<PauseTime>03:00:00</PauseTime>", xmlText);
        }
    }
}
