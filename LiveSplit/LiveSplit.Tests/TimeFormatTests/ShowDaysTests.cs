using System;
using LiveSplit.TimeFormatters;
using Xunit;

namespace LiveSplit.Tests.TimeFormatTests
{
    public class ShowDaysTests
    {
        [Theory]
        [InlineData("00:00:01", "1", DigitsFormat.SingleDigitSeconds)]
        [InlineData("00:00:02", "02", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("00:00:03", "0:03", DigitsFormat.SingleDigitMinutes)]
        [InlineData("00:00:04", "00:04", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("00:00:05", "0:00:05", DigitsFormat.SingleDigitHours)]
        [InlineData("00:00:06", "00:00:06", DigitsFormat.DoubleDigitHours)]

        [InlineData("1:02:51:12", "1d 2:51:12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("1:03:42:34", "1d 3:42:34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("1:04:33:56", "1d 4:33:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("1:05:24:23", "1d 5:24:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("1:01:15:45", "1d 1:15:45", DigitsFormat.SingleDigitHours)]
        [InlineData("1:02:56:51", "1d 02:56:51", DigitsFormat.DoubleDigitHours)]

        [InlineData("1:00:00:00", "1d 0:00:00", DigitsFormat.SingleDigitHours)]
        [InlineData("3:00:00:00", "3d 00:00:00", DigitsFormat.DoubleDigitHours)]

        [InlineData("2:00:00:23", "2d 0:00:23", DigitsFormat.SingleDigitHours)]
        [InlineData("4:00:00:23", "4d 00:00:23", DigitsFormat.DoubleDigitHours)]

        [InlineData("-5:01:15:45", "−5d 1:15:45", DigitsFormat.SingleDigitHours)]
        [InlineData("-6:02:56:51", "−6d 02:56:51", DigitsFormat.DoubleDigitHours)]
        public void TestShowDays(string timespanText, string expected, DigitsFormat format)
        {
            var formatter = new GeneralTimeFormatter();
            formatter.DigitsFormat = format;
            formatter.ShowDays = true;
            formatter.Accuracy = TimeAccuracy.Seconds;

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }
    }
}
