﻿using System;

using LiveSplit.TimeFormatters;

using Xunit;

namespace LiveSplit.Tests.TimeFormatterTests;

public class RegularTimeFormattersTests
{
    // These tests cover the following, which are/were all based on RegularTimeFormatter:
    //new RegularTimeFormatter(timeAccuracy);
    //new AutomaticPrecisionTimeFormatter(); // -> RegularTimeFormatter with changes

    [Theory]
    [InlineData(TimeAccuracy.Seconds, "0")]
    [InlineData(TimeAccuracy.Tenths, "0.0")]
    [InlineData(TimeAccuracy.Hundredths, "0.00")]
    public void RegularTimeFormatterFormatsTimeCorrectlyInGivenAccuracy_WhenTimeIsNull(TimeAccuracy timeAccuracy, string expectedTime)
    {
        var sut = new RegularTimeFormatter(timeAccuracy);

        string formattedTime = sut.Format(null);
        Assert.Equal(expectedTime, formattedTime);
    }

    [Theory]
    [InlineData("00:00:00", TimeAccuracy.Seconds, "0:00")]
    [InlineData("00:00:00", TimeAccuracy.Tenths, "0:00.0")]
    [InlineData("00:00:00", TimeAccuracy.Hundredths, "0:00.00")]
    [InlineData("00:00:01", TimeAccuracy.Seconds, "0:01")]
    [InlineData("00:00:00.5", TimeAccuracy.Tenths, "0:00.5")]
    [InlineData("00:00:01.001", TimeAccuracy.Tenths, "0:01.0")]
    [InlineData("00:00:01.009", TimeAccuracy.Tenths, "0:01.0")]
    [InlineData("00:05:01.999", TimeAccuracy.Tenths, "5:01.9")]
    [InlineData("00:25:01.999", TimeAccuracy.Tenths, "25:01.9")]
    [InlineData("01:05:01.999", TimeAccuracy.Tenths, "1:05:01.9")]
    [InlineData("00:00:00.05", TimeAccuracy.Hundredths, "0:00.05")]
    [InlineData("00:10:00.006", TimeAccuracy.Hundredths, "10:00.00")]
    [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, "227:01:01.99")]
    [InlineData("1.00:00:01.999", TimeAccuracy.Hundredths, "24:00:01.99")]
    public void RegularTimeFormatterFormatsTimeCorrectlyInGivenAccuracy_WhenTimeIsValid(string timespanText, TimeAccuracy timeAccuracy, string expectedTime)
    {
        var sut = new RegularTimeFormatter(timeAccuracy);
        var time = TimeSpan.Parse(timespanText);

        string formattedTime = sut.Format(time);
        Assert.Equal(expectedTime, formattedTime);
    }

    [Fact]
    public void AutomaticPrecisionTimeFormatterFormatsTimeAsZero_WhenTimeIsNull()
    {
        var sut = new AutomaticPrecisionTimeFormatter();

        string formattedTime = sut.Format(null);
        Assert.Equal("0", formattedTime);
    }

    [Theory]
    [InlineData("00:00:00", "0:00")]
    [InlineData("00:00:01", "0:01")]
    [InlineData("00:00:02.0", "0:02")]
    [InlineData("00:00:03.00", "0:03")]
    [InlineData("00:00:01.2", "0:01.2")]
    [InlineData("00:00:01.20", "0:01.2")]
    [InlineData("00:00:01.200", "0:01.2")]
    [InlineData("00:00:01.23", "0:01.23")]
    [InlineData("00:00:01.234", "0:01.23")]
    [InlineData("00:00:01.2345", "0:01.23")]
    [InlineData("00:00:01.2000", "0:01.2")]
    [InlineData("00:00:01.204", "0:01.20")]
    [InlineData("00:00:01.2005", "0:01.20")]
    [InlineData("00:05:01.999", "5:01.99")]
    [InlineData("00:25:01.999", "25:01.99")]
    [InlineData("00:10:00.006", "10:00.00")]
    public void AutomaticPrecisionTimeFormatterFormatsTimeCorrectly_WhenTimeIsValid(string timespanText, string expectedTime)
    {
        var sut = new AutomaticPrecisionTimeFormatter();
        var time = TimeSpan.Parse(timespanText);

        string formattedTime = sut.Format(time);
        Assert.Equal(expectedTime, formattedTime);
    }

    [Theory]
    [InlineData("-00:00:00.05", TimeAccuracy.Hundredths, "−0:00.05")]
    [InlineData("-00:00:01.009", TimeAccuracy.Tenths, "−0:01.0")]
    [InlineData("-00:05:01.999", TimeAccuracy.Tenths, "−5:01.9")]
    [InlineData("-9.12:09:01.999", TimeAccuracy.Hundredths, "−228:09:01.99")]
    [InlineData("-1.00:02:01.999", TimeAccuracy.Hundredths, "−24:02:01.99")]
    public void NegativesTestRegularTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, string expectedTime)
    {
        var formatter = new RegularTimeFormatter(timeAccuracy);
        var time = TimeSpan.Parse(timespanText);

        string formattedTime = formatter.Format(time);
        Assert.Equal(expectedTime, formattedTime);
    }
}
