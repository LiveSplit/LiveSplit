using System;
using Xunit;
using LiveSplit.TimeFormatters;

namespace LiveSplit.Tests.TimeFormatTests
{
    public class ShowDaysTests
    {
        [Theory]
        [InlineData("00:00:01", DigitsFormat.SingleDigitSeconds, "1")]
        [InlineData("00:00:02", DigitsFormat.DoubleDigitSeconds, "02")]
        [InlineData("00:00:03", DigitsFormat.SingleDigitMinutes, "0:03")]
        [InlineData("00:00:04", DigitsFormat.DoubleDigitMinutes, "00:04")]
        [InlineData("00:00:05", DigitsFormat.SingleDigitHours, "0:00:05")]
        [InlineData("00:00:06", DigitsFormat.DoubleDigitHours, "00:00:06")]

        [InlineData("1:02:51:12", DigitsFormat.SingleDigitSeconds, "1d 2:51:12")]
        [InlineData("1:03:42:34", DigitsFormat.DoubleDigitSeconds, "1d 3:42:34")]
        [InlineData("1:04:33:56", DigitsFormat.SingleDigitMinutes, "1d 4:33:56")]
        [InlineData("1:05:24:23", DigitsFormat.DoubleDigitMinutes, "1d 5:24:23")]
        [InlineData("1:01:15:45", DigitsFormat.SingleDigitHours, "1d 1:15:45")]
        [InlineData("1:02:56:51", DigitsFormat.DoubleDigitHours, "1d 02:56:51")]

        [InlineData("1:00:00:00", DigitsFormat.SingleDigitHours, "1d 0:00:00")]
        [InlineData("3:00:00:00", DigitsFormat.DoubleDigitHours, "3d 00:00:00")]

        [InlineData("2:00:00:23", DigitsFormat.SingleDigitHours, "2d 0:00:23")]
        [InlineData("4:00:00:23", DigitsFormat.DoubleDigitHours, "4d 00:00:23")]

        [InlineData("-5:01:15:45", DigitsFormat.SingleDigitHours, "−5d 1:15:45")]
        [InlineData("-6:02:56:51", DigitsFormat.DoubleDigitHours, "−6d 02:56:51")]
        public void FormatsTimCorrectly_WhenShowDaysIsTrue(string timespanText, DigitsFormat format, string expectedTime)
        {
            var sut = new GeneralTimeFormatter
            {
                DigitsFormat = format,
                ShowDays = true,
                Accuracy = TimeAccuracy.Seconds
            };

            var time = TimeSpan.Parse(timespanText);
            var formattedTime = sut.Format(time);

            Assert.Equal(expectedTime, formattedTime);
        }
    }
}