using System;
using Xunit;
using LiveSplit.TimeFormatters;

namespace LiveSplit.Tests.TimeFormatTests
{
    public class DigitsFormatTests
    {
        [Theory]
        [InlineData("00:00:00", DigitsFormat.SingleDigitSeconds, "0")]
        [InlineData("00:00:00", DigitsFormat.DoubleDigitSeconds, "00")]
        [InlineData("00:00:00", DigitsFormat.SingleDigitMinutes, "0:00")]
        [InlineData("00:00:00", DigitsFormat.DoubleDigitMinutes, "00:00")]
        [InlineData("00:00:00", DigitsFormat.SingleDigitHours, "0:00:00")]
        [InlineData("00:00:00", DigitsFormat.DoubleDigitHours, "00:00:00")]

        [InlineData("00:00:01", DigitsFormat.SingleDigitSeconds, "1")]
        [InlineData("00:00:02", DigitsFormat.DoubleDigitSeconds, "02")]
        [InlineData("00:00:03", DigitsFormat.SingleDigitMinutes, "0:03")]
        [InlineData("00:00:04", DigitsFormat.DoubleDigitMinutes, "00:04")]
        [InlineData("00:00:05", DigitsFormat.SingleDigitHours, "0:00:05")]
        [InlineData("00:00:06", DigitsFormat.DoubleDigitHours, "00:00:06")]

        [InlineData("00:00:12", DigitsFormat.SingleDigitSeconds, "12")]
        [InlineData("00:00:34", DigitsFormat.DoubleDigitSeconds, "34")]
        [InlineData("00:00:56", DigitsFormat.SingleDigitMinutes, "0:56")]
        [InlineData("00:00:23", DigitsFormat.DoubleDigitMinutes, "00:23")]
        [InlineData("00:00:45", DigitsFormat.SingleDigitHours, "0:00:45")]
        [InlineData("00:00:51", DigitsFormat.DoubleDigitHours, "00:00:51")]

        [InlineData("00:01:12", DigitsFormat.SingleDigitSeconds, "1:12")]
        [InlineData("00:02:34", DigitsFormat.DoubleDigitSeconds, "2:34")]
        [InlineData("00:03:56", DigitsFormat.SingleDigitMinutes, "3:56")]
        [InlineData("00:04:23", DigitsFormat.DoubleDigitMinutes, "04:23")]
        [InlineData("00:05:45", DigitsFormat.SingleDigitHours, "0:05:45")]
        [InlineData("00:06:51", DigitsFormat.DoubleDigitHours, "00:06:51")]

        [InlineData("00:51:12", DigitsFormat.SingleDigitSeconds, "51:12")]
        [InlineData("00:42:34", DigitsFormat.DoubleDigitSeconds, "42:34")]
        [InlineData("00:33:56", DigitsFormat.SingleDigitMinutes, "33:56")]
        [InlineData("00:24:23", DigitsFormat.DoubleDigitMinutes, "24:23")]
        [InlineData("00:15:45", DigitsFormat.SingleDigitHours, "0:15:45")]
        [InlineData("00:56:51", DigitsFormat.DoubleDigitHours, "00:56:51")]

        [InlineData("02:51:12", DigitsFormat.SingleDigitSeconds, "2:51:12")]
        [InlineData("03:42:34", DigitsFormat.DoubleDigitSeconds, "3:42:34")]
        [InlineData("04:33:56", DigitsFormat.SingleDigitMinutes, "4:33:56")]
        [InlineData("05:24:23", DigitsFormat.DoubleDigitMinutes, "5:24:23")]
        [InlineData("01:15:45", DigitsFormat.SingleDigitHours, "1:15:45")]
        [InlineData("02:56:51", DigitsFormat.DoubleDigitHours, "02:56:51")]

        [InlineData("22:51:12", DigitsFormat.SingleDigitSeconds, "22:51:12")]
        [InlineData("23:42:34", DigitsFormat.DoubleDigitSeconds, "23:42:34")]
        [InlineData("14:33:56", DigitsFormat.SingleDigitMinutes, "14:33:56")]
        [InlineData("15:24:23", DigitsFormat.DoubleDigitMinutes, "15:24:23")]
        [InlineData("21:15:45", DigitsFormat.SingleDigitHours, "21:15:45")]
        [InlineData("22:56:51", DigitsFormat.DoubleDigitHours, "22:56:51")]
        
        [InlineData("1:22:51:12", DigitsFormat.SingleDigitSeconds, "46:51:12")]
        [InlineData("1:23:42:34", DigitsFormat.DoubleDigitSeconds, "47:42:34")]
        [InlineData("1:14:33:56", DigitsFormat.SingleDigitMinutes, "38:33:56")]
        [InlineData("1:15:24:23", DigitsFormat.DoubleDigitMinutes, "39:24:23")]
        [InlineData("1:21:15:45", DigitsFormat.SingleDigitHours, "45:15:45")]
        [InlineData("1:22:56:51", DigitsFormat.DoubleDigitHours, "46:56:51")]
        public void ConvertToExpectedValue_WhenAValidTimespanTextIsFormatted(string timespanText, DigitsFormat format, string expectedTime)
        {
            var sut = new GeneralTimeFormatter
            {
                DigitsFormat = format,
                Accuracy = TimeAccuracy.Seconds
            };

            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedTime, formattedTime);
        }

        [Fact]
        public void ReturnDash_WhenFormattingNullTime()
        {
            var sut = new GeneralTimeFormatter();

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }

        [Theory]
        [InlineData(NullFormat.ZeroDotZeroZero, "0.00")]
        [InlineData(NullFormat.ZeroValue, "0")]
        [InlineData(NullFormat.Dashes, "-")]
        public void ReturnExpectedValues_WhenFormattingNullTimeAndDeterminedNullFormatsAreExpected(
            NullFormat nullFormat, string expectedConversion)
        {
            var sut = new GeneralTimeFormatter
            {
                NullFormat = nullFormat
            };

            var formattedTime = sut.Format(null);
            Assert.Equal(expectedConversion, formattedTime);
        }

        [Theory]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Seconds, "0")]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Tenths, "0.0")]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Hundredths, "0.00")]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Milliseconds, "0.000")]
        public void ReturnZeroWithCorrectAmountOfDecimals_WhenZeroWithAccuracyIsExpected(
            NullFormat nullFormat, TimeAccuracy accuracy, string expectedConversion)
        {
            var sut = new GeneralTimeFormatter
            {
                NullFormat = nullFormat,
                Accuracy = accuracy
            };

            var formattedTime = sut.Format(null);
            Assert.Equal(expectedConversion, formattedTime);
        }
    }
}