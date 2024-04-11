using System;
using Xunit;
using LiveSplit.Model;
using static LiveSplit.Tests.Model.Constants;
using System.Xml;

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

        [Fact]
        public void SerializeToXmlCorrectly()
        {
            var anyTime = new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(YetAnotherTickValue));
            var anyIndex = 1;
            var sut = new IndexedTime(anyTime, anyIndex);

            var document = new XmlDocument();
            var element = sut.ToXml(document);
            var xmlText = element.OuterXml;

            Assert.Equal("Time", element.Name);
            Assert.Equal("1", element.Attributes["id"].Value);
            Assert.Contains("<RealTime>9.00:00:00</RealTime>", xmlText);
            Assert.Contains("<GameTime>8.23:33:20</GameTime>", xmlText);
        }

        [Fact]
        public void DeserializeFromXmlCorrectly()
        {
            var xml = "<Time id=\"1\"><RealTime>9.00:00:00</RealTime><GameTime>8.23:33:20</GameTime></Time>";
            var document = new XmlDocument();
            document.LoadXml(xml);

            var sut = IndexedTimeHelper.ParseXml(document.DocumentElement);
            Assert.Equal(1, sut.Index);
            Assert.Equal(AnyTimeSpan, sut.Time.RealTime);
            Assert.Equal(YetAnotherTimeSpan, sut.Time.GameTime);
        }

        [Fact]
        public void ReturnNull_WhenDeserializingNullOldXml() =>
            Assert.Throws<NullReferenceException>(() => IndexedTimeHelper.ParseXmlOld(null));

        [Fact]
        public void DeserializeOldXmlCorrectly()
        {
            var xml = $@"<Time id=""3"">9.00:00:00 | 8.23:33:20</Time>";
            var document = new XmlDocument();
            document.LoadXml(xml);

            var sut = IndexedTimeHelper.ParseXmlOld(document.DocumentElement);
            Assert.Equal(3, sut.Index);
            Assert.Equal(AnyTimeSpan, sut.Time.RealTime);
            Assert.Equal(YetAnotherTimeSpan, sut.Time.GameTime);
        }

        [Fact]
        public void SerializeToJsonCorrectly()
        {
            var anyTime = new Time(TimeSpan.FromTicks(AnyTickValue), TimeSpan.FromTicks(YetAnotherTickValue));
            var anyIndex = 1;
            var sut = new IndexedTime(anyTime, anyIndex);

            dynamic json = IndexedTimeHelper.ToJson(sut);

            Assert.Equal(1, json.id);
            Assert.Equal(AnyTimeSpan, json.realTime);
            Assert.Equal(YetAnotherTimeSpan, json.gameTime);
        }
    }
}
