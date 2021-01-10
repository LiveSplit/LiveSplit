using System;
using LiveSplit.TimeFormatters;
using Xunit;

namespace LiveSplit.Tests.TimeFormatTests
{
    public class DigitsFormatTests
    {
        [Theory]
        [InlineData("00:00:00", "0", DigitsFormat.SingleDigitSeconds)]
        [InlineData("00:00:00", "00", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("00:00:00", "0:00", DigitsFormat.SingleDigitMinutes)]
        [InlineData("00:00:00", "00:00", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("00:00:00", "0:00:00", DigitsFormat.SingleDigitHours)]
        [InlineData("00:00:00", "00:00:00", DigitsFormat.DoubleDigitHours)]

        [InlineData("00:00:01", "1", DigitsFormat.SingleDigitSeconds)]
        [InlineData("00:00:02", "02", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("00:00:03", "0:03", DigitsFormat.SingleDigitMinutes)]
        [InlineData("00:00:04", "00:04", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("00:00:05", "0:00:05", DigitsFormat.SingleDigitHours)]
        [InlineData("00:00:06", "00:00:06", DigitsFormat.DoubleDigitHours)]

        [InlineData("00:00:12", "12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("00:00:34", "34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("00:00:56", "0:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("00:00:23", "00:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("00:00:45", "0:00:45", DigitsFormat.SingleDigitHours)]
        [InlineData("00:00:51", "00:00:51", DigitsFormat.DoubleDigitHours)]

        [InlineData("00:01:12", "1:12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("00:02:34", "2:34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("00:03:56", "3:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("00:04:23", "04:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("00:05:45", "0:05:45", DigitsFormat.SingleDigitHours)]
        [InlineData("00:06:51", "00:06:51", DigitsFormat.DoubleDigitHours)]

        [InlineData("00:51:12", "51:12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("00:42:34", "42:34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("00:33:56", "33:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("00:24:23", "24:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("00:15:45", "0:15:45", DigitsFormat.SingleDigitHours)]
        [InlineData("00:56:51", "00:56:51", DigitsFormat.DoubleDigitHours)]

        [InlineData("02:51:12", "2:51:12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("03:42:34", "3:42:34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("04:33:56", "4:33:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("05:24:23", "5:24:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("01:15:45", "1:15:45", DigitsFormat.SingleDigitHours)]
        [InlineData("02:56:51", "02:56:51", DigitsFormat.DoubleDigitHours)]

        [InlineData("22:51:12", "22:51:12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("23:42:34", "23:42:34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("14:33:56", "14:33:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("15:24:23", "15:24:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("21:15:45", "21:15:45", DigitsFormat.SingleDigitHours)]
        [InlineData("22:56:51", "22:56:51", DigitsFormat.DoubleDigitHours)]
        
        [InlineData("1:22:51:12", "46:51:12", DigitsFormat.SingleDigitSeconds)]
        [InlineData("1:23:42:34", "47:42:34", DigitsFormat.DoubleDigitSeconds)]
        [InlineData("1:14:33:56", "38:33:56", DigitsFormat.SingleDigitMinutes)]
        [InlineData("1:15:24:23", "39:24:23", DigitsFormat.DoubleDigitMinutes)]
        [InlineData("1:21:15:45", "45:15:45", DigitsFormat.SingleDigitHours)]
        [InlineData("1:22:56:51", "46:56:51", DigitsFormat.DoubleDigitHours)]
        public void ConvertToExpectedValue_WhenAValidTimespanTextIsFormatted(string timespanText, string expected, DigitsFormat format)
        {
            var formatter = new GeneralTimeFormatter
            {
                DigitsFormat = format,
                Accuracy = TimeAccuracy.Seconds
            };

            var time = TimeSpan.Parse(timespanText);

            var formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }

        [Fact]
        public void ReturnDash_WhenFormattingNullTime()
        {
            var formatter = new GeneralTimeFormatter();

            var formatted = formatter.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formatted);
        }

        [Theory]
        [InlineData(NullFormat.ZeroDotZeroZero, "0.00")]
        [InlineData(NullFormat.ZeroValue, "0")]
        [InlineData(NullFormat.Dashes, "-")]
        public void ReturnExpectedValues_WhenFormattingNullTimeAndDeterminedNullFormatsAreExpected(
            NullFormat nullFormat, string expectedConversion)
        {
            var formatter = new GeneralTimeFormatter
            {
                NullFormat = nullFormat
            };

            var formatted = formatter.Format(null);
            Assert.Equal(expectedConversion, formatted);
        }

        [Theory]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Seconds, "0")]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Tenths, "0.0")]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Hundredths, "0.00")]
        [InlineData(NullFormat.ZeroWithAccuracy, TimeAccuracy.Milliseconds, "0.000")]
        public void ReturnZeroWithCorrectAmountOfDecimals_WhenZeroWithAccuracyIsExpected(
            NullFormat nullFormat, TimeAccuracy accuracy, string expectedConversion)
        {
            var formatter = new GeneralTimeFormatter
            {
                NullFormat = nullFormat,
                Accuracy = accuracy
            };

            var formatted = formatter.Format(null);
            Assert.Equal(expectedConversion, formatted);
        }
    }
}