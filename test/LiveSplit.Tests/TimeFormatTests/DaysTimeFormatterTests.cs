﻿using System;

using LiveSplit.TimeFormatters;

using Xunit;

namespace LiveSplit.Tests.TimeFormatterTests;

public class DaysTimeFormatterTests
{
    // These tests cover DaysTimeFormatter, which (is/was not) based on any other TimeFormatter implementation
    [Fact]
    public void FormatsTimeAsZero_WhenTimeIsNull()
    {
        var sut = new DaysTimeFormatter();

        string formattedTime = sut.Format(null);
        Assert.Equal("0", formattedTime);
    }

    [Theory]
    [InlineData("00:00:00", "0:00")]
    [InlineData("00:00:01.03", "0:01")]
    [InlineData("00:05:01.03", "5:01")]
    [InlineData("07:05:01.03", "7:05:01")]
    [InlineData("1.07:05:01.03", "1d 7:05:01")]
    [InlineData("9.23:02:03.412", "9d 23:02:03")]
    [InlineData("272.13:04:05.612", "272d 13:04:05")]
    public void TestDaysTimeFormatter(string timespanText, string expectedTime)
    {
        var sut = new DaysTimeFormatter();
        var time = TimeSpan.Parse(timespanText);

        string formattedTime = sut.Format(time);
        Assert.Equal(expectedTime, formattedTime);
    }

    /*
    // These tests will fail until changes are made.
    // Currently:
    // - negative TimeSpans are displayed as positive
    // - negative days + hours are not displayed
    [Theory]
    [InlineData("-00:05:01.03", "−5:01")] // Actual:<5:01> [Fail]
    [InlineData("-1.00:00:01.999", "−1d 0:00:01")] // Actual:<0:01> [Fail]
    [InlineData("-9.23:02:03.412", "−9d 23:02:03")] // Actual:<2:03>. [Fail]
    [InlineData("-222.13:04:05.612", "−222d 13:04:05")] // Actual:<4:05>. [Fail]
    public void NegativeTestDaysTimeFormatter(string timespanText, string expected) {
    var formatter = new DaysTimeFormatter();

        TimeSpan? time = null;
        if (timespanText != null)
            time = TimeSpan.Parse(timespanText);

        string formatted = formatter.Format(time);
        Assert.Equal(expected, formatted);
    }
    */

}
