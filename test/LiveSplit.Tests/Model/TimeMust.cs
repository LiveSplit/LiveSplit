using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Xml;
using Xunit;
using static LiveSplit.Tests.Model.Constants;

namespace LiveSplit.Tests.Model;

public class TimeMust
{
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
        object[] possibleValues = [null, TimeSpan.Zero];
        foreach (object anyRealTime in possibleValues)
        {
            foreach (object anyGameTime in possibleValues)
            {
                yield return [anyRealTime, anyGameTime];
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
        yield return [AnyTimeSpan, AnotherTimeSpan, "9.00:00:00 | 03:00:00"];
        yield return [null, AnotherTimeSpan, " | 03:00:00"];
        yield return [AnyTimeSpan, null, "9.00:00:00 | "];
        yield return [null, null, " | "];
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
        yield return ["9.00:00:00 | 03:00:00", AnyTimeSpan, AnotherTimeSpan];
        yield return ["| 03:00:00", null, AnotherTimeSpan];
        yield return ["9.00:00:00 | ", AnyTimeSpan, null];
        yield return ["|", null, null];
        yield return [string.Empty, null, null];
        yield return ["invalid|4:1.0", null, null];
    }

    [Theory]
    [MemberData(nameof(JsonSerializationFeeder))]
    public void SerializeToJsonCorrectly(TimeSpan? realTime, TimeSpan? gameTime, string expectedRealTimeText, string expectedGameTimeText)
    {
        var sut = new Time(realTime, gameTime);
        string json = sut.ToJson().ToString();
        Assert.Matches(expectedRealTimeText, json);
        Assert.Matches(expectedGameTimeText, json);
    }

    public static IEnumerable<object[]> JsonSerializationFeeder()
    {
        yield return [AnyTimeSpan, AnotherTimeSpan, @"realTime""\s*:\s*""9.00:00:00""", @"gameTime""\s*:\s*""03:00:00"""];
        yield return [null, AnotherTimeSpan, @"realTime""\s*:\s*""""", @"gameTime""\s*:\s*""03:00:00"""];
        yield return [AnyTimeSpan, null, @"realTime""\s*:\s*""9.00:00:00""", @"gameTime""\s*:\s*"""""];
        yield return [null, null, @"realTime""\s*:\s*""""", @"gameTime""\s*:\s*"""""];
    }

    [Fact]
    public void SerializeToDefaultXmlRootCorrectly()
    {
        var document = new XmlDocument();
        var sut = new Time(AnyTimeSpan, AnotherTimeSpan);
        XmlElement root = sut.ToXml(document);
        Assert.Equal("Time", root.Name);
    }

    [Fact]
    public void SerializeToGivenXmlRootCorrectly()
    {
        string anyRoot = "AnyRoot";
        var document = new XmlDocument();
        var sut = new Time(AnyTimeSpan, AnotherTimeSpan);
        XmlElement root = sut.ToXml(document, anyRoot);
        Assert.Equal(anyRoot, root.Name);
    }

    [Theory]
    [MemberData(nameof(XmlSerializationFeeder))]
    public void SerializeToXmlCorrectly(TimeSpan? realTime, TimeSpan? gameTime, string expectedRealTimeText, string expectedGameTimeText)
    {
        var document = new XmlDocument();
        var sut = new Time(realTime, gameTime);
        string innerXmlAsText = sut.ToXml(document).InnerXml;
        Assert.Contains(expectedRealTimeText, innerXmlAsText);
        Assert.Contains(expectedGameTimeText, innerXmlAsText);
    }

    public static IEnumerable<object[]> XmlSerializationFeeder()
    {
        yield return [AnyTimeSpan, AnotherTimeSpan, "<RealTime>9.00:00:00</RealTime>", "<GameTime>03:00:00</GameTime>"];
        yield return [null, AnotherTimeSpan, string.Empty, "<GameTime>03:00:00</GameTime>"];
        yield return [AnyTimeSpan, null, "<RealTime>9.00:00:00</RealTime>", string.Empty];
        yield return [null, null, string.Empty, string.Empty];
    }

    [Theory]
    [MemberData(nameof(XmlDeserializationFeeder))]
    public void DeserializeFromXmlCorrectly(string xml, TimeSpan? expectedRealTime, TimeSpan? expectedGameTime)
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        var sut = Time.FromXml(document.DocumentElement);
        Assert.Equal(expectedRealTime, sut.RealTime);
        Assert.Equal(expectedGameTime, sut.GameTime);
    }

    public static IEnumerable<object[]> XmlDeserializationFeeder()
    {
        yield return ["<Time><RealTime>9.00:00:00</RealTime><GameTime>03:00:00</GameTime></Time>", AnyTimeSpan, AnotherTimeSpan];
        yield return ["<Time><GameTime>03:00:00</GameTime></Time>", null, AnotherTimeSpan];
        yield return ["<Time><RealTime>9.00:00:00</RealTime></Time>", AnyTimeSpan, null];
        yield return ["<Time></Time>", null, null];
    }

    [Theory]
    [MemberData(nameof(SubstractionFeeder))]
    public void SubstractTimeCorrectly(Time minuend, Time substrahend, Time difference)
    {
        Time sut = minuend - substrahend;
        Assert.Equal(difference, sut);
    }

    public static IEnumerable<object[]> SubstractionFeeder()
    {
        yield return [new Time(AnyTimeSpan, YetAnotherTimeSpan), new Time(AnotherTimeSpan, YetAnotherTimeSpan), new Time(TimeSpan.FromTicks(Difference), TimeSpan.FromTicks(0))];
        yield return [new Time(AnyTimeSpan, YetAnotherTimeSpan), new Time(AnotherTimeSpan), new Time(TimeSpan.FromTicks(Difference))];
        yield return [new Time(YetAnotherTimeSpan, AnyTimeSpan), new Time(YetAnotherTimeSpan, AnotherTimeSpan), new Time(TimeSpan.FromTicks(0), TimeSpan.FromTicks(Difference))];
        yield return [new Time(YetAnotherTimeSpan, AnyTimeSpan), new Time(null, AnotherTimeSpan), new Time(null, TimeSpan.FromTicks(Difference))];
        yield return [new Time(AnotherTimeSpan, YetAnotherTimeSpan), new Time(AnotherTimeSpan, YetAnotherTimeSpan), Time.Zero];
        yield return [Time.Zero, new Time(AnyTimeSpan, AnotherTimeSpan), new Time(TimeSpan.FromTicks(-AnyTickValue), TimeSpan.FromHours(-3))];
    }

    [Theory]
    [MemberData(nameof(AdditionFeeder))]
    public void AddTimeCorrectly(Time leftAddend, Time rightAddend, Time sum)
    {
        Time sut = leftAddend + rightAddend;
        Assert.Equal(sum, sut);
    }

    public static IEnumerable<object[]> AdditionFeeder()
    {
        yield return [Time.Zero, Time.Zero, Time.Zero];
        yield return [new Time(AnyTimeSpan, YetAnotherTimeSpan), Time.Zero, new Time(AnyTimeSpan, YetAnotherTimeSpan)];
        yield return [Time.Zero, new Time(AnyTimeSpan, YetAnotherTimeSpan), new Time(AnyTimeSpan, YetAnotherTimeSpan)];
        yield return [new Time(AnyTimeSpan, YetAnotherTimeSpan), new Time(AnotherTimeSpan), new Time(TimeSpan.FromTicks(Addition))];
        yield return [new Time(null, AnotherTimeSpan), new Time(YetAnotherTimeSpan, AnyTimeSpan), new Time(null, TimeSpan.FromTicks(Addition))];

    }
}
