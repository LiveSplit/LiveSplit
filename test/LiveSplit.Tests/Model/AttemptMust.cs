using System;
using System.Collections.Generic;
using System.Xml;
using Xunit;
using LiveSplit.Model;
using static LiveSplit.Tests.Model.Constants;

namespace LiveSplit.Tests.Model
{
    public class AttemptMust
    {
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
            Assert.Equal("12/13/2020 00:00:00", sut.Attributes["started"].Value);
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
            var attempt = new Attempt(2, new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(AnyTickValue)), null, new AtomicDateTime(AnyDateTime, false), null);

            var sut = attempt.ToXml(document);
            var xmlText = sut.OuterXml;

            Assert.Equal("2", sut.Attributes["id"].Value);
            Assert.Null(sut.Attributes["started"]);
            Assert.Equal("12/13/2020 00:00:00", sut.Attributes["ended"].Value);
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
            var attempt = new Attempt(3, new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(AnyTickValue)), new AtomicDateTime(AnyDateTime, true), new AtomicDateTime(AnyDateTime, false), AnotherTimeSpan);

            var sut = attempt.ToXml(document);
            var xmlText = sut.OuterXml;

            Assert.Equal("3", sut.Attributes["id"].Value);
            Assert.Equal("12/13/2020 00:00:00", sut.Attributes["started"].Value);
            Assert.Equal("12/13/2020 00:00:00", sut.Attributes["ended"].Value);
            Assert.Equal("True", sut.Attributes["isStartedSynced"].Value);
            Assert.Equal("False", sut.Attributes["isEndedSynced"].Value);
            Assert.Contains("<RealTime>9.00:00:00</RealTime>", xmlText);
            Assert.Contains("<GameTime>9.00:00:00</GameTime>", xmlText);
            Assert.Contains("<PauseTime>03:00:00</PauseTime>", xmlText);
        }

        [Fact]
        public void DeserializeToXmlCorrectly_WhenAttemptIsEmpty()
        {
            var xml = "<Attempt id=\"0\"><RealTime>00:00:00</RealTime><GameTime>00:00:00</GameTime></Attempt>";
            var document = new XmlDocument();
            document.LoadXml(xml);

            var sut = Attempt.ParseXml(document.DocumentElement);
            Assert.Equal(0, sut.Index);
            Assert.Null(sut.Started);
            Assert.Null(sut.Ended);
            Assert.Equal(TimeSpan.Zero, sut.Time.RealTime);
            Assert.Equal(TimeSpan.Zero, sut.Time.GameTime);
            Assert.Null(sut.PauseTime);
        }

        [Fact]
        public void DeserializeToXmlCorrectly_WhenAttemptHasStartedTime()
        {
            var xml = "<Attempt id=\"1\" started=\"12/13/2020 00:00:00\" isStartedSynced=\"True\"><RealTime>9.00:00:00</RealTime></Attempt>";
            var document = new XmlDocument();
            document.LoadXml(xml);

            var sut = Attempt.ParseXml(document.DocumentElement);
            Assert.Equal(1, sut.Index);
            Assert.Equal(TimeSpan.FromTicks(AnyTickValue), sut.Time.RealTime);
            Assert.Null(sut.Time.GameTime);
            Assert.True(sut.Started.Value.SyncedWithAtomicClock);
            Assert.Equal(AnyDateTime.ToUniversalTime(), sut.Started.Value.Time.ToUniversalTime());
            Assert.Null(sut.PauseTime);
        }

        [Fact]
        public void DeserializeToXmlCorrectly_WhenAttemptHasEndedTime()
        {
            var xml = "<Attempt id=\"2\" ended=\"12/13/2020 00:00:00\" isEndedSynced=\"False\"><RealTime>9.00:00:00</RealTime><GameTime>9.00:00:00</GameTime></Attempt>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var sut = Attempt.ParseXml(document.DocumentElement);

            Assert.Equal(2, sut.Index);
            Assert.Null(sut.Started);
            Assert.Equal(AnyDateTime.ToUniversalTime(), sut.Ended.Value.Time.ToUniversalTime());
            Assert.False(sut.Ended.Value.SyncedWithAtomicClock);
            Assert.Equal(AnyTimeSpan, sut.Time.RealTime);
            Assert.Equal(AnyTimeSpan, sut.Time.GameTime);
            Assert.Null(sut.PauseTime);
        }

        [Fact]
        public void DeserializeFromXmlCorrectly_WhenXmlIncludesPauseTime()
        {
            var xml = "<Attempt id=\"3\" started=\"12/13/2020 00:00:00\" isStartedSynced=\"True\" ended=\"12/13/2020 00:00:00\" isEndedSynced=\"False\"><RealTime>9.00:00:00</RealTime><GameTime>9.00:00:00</GameTime><PauseTime>03:00:00</PauseTime></Attempt>";
            var document = new XmlDocument();
            document.LoadXml(xml);

            var sut = Attempt.ParseXml(document.DocumentElement);
            Assert.Equal(3, sut.Index);
            Assert.Equal(AnyDateTime.ToUniversalTime(), sut.Started.Value.Time.ToUniversalTime());
            Assert.Equal(AnyDateTime.ToUniversalTime(), sut.Ended.Value.Time.ToUniversalTime());
            Assert.True(sut.Started.Value.SyncedWithAtomicClock);
            Assert.False(sut.Ended.Value.SyncedWithAtomicClock);
            Assert.Equal(AnyTimeSpan, sut.Time.RealTime);
            Assert.Equal(AnyTimeSpan, sut.Time.GameTime);
            Assert.Equal(AnotherTimeSpan, sut.PauseTime);
        }
    }
}
