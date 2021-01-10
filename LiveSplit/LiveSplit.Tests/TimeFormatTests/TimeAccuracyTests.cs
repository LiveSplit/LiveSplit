using System;
using LiveSplit.TimeFormatters;
using Xunit;

namespace LiveSplit.Tests.TimeFormatTests
{
    public class TimeAccuracyTests
    {
        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, "0:00")]
        [InlineData("00:00:01", TimeAccuracy.Tenths, "0:01.0")]
        [InlineData("00:00:02", TimeAccuracy.Hundredths, "0:02.00")]
        [InlineData("00:00:03", TimeAccuracy.Milliseconds, "0:03.000")]

        [InlineData("00:00:04", TimeAccuracy.Seconds, "0:04")]
        [InlineData("00:00:05.2", TimeAccuracy.Tenths, "0:05.2")]
        [InlineData("00:00:06.22", TimeAccuracy.Hundredths, "0:06.22")]
        [InlineData("00:00:07.222", TimeAccuracy.Milliseconds, "0:07.222")]

        [InlineData("00:00:08.8888888", TimeAccuracy.Seconds, "0:08")]
        [InlineData("00:00:09.8888888", TimeAccuracy.Tenths, "0:09.8")]
        [InlineData("00:00:10.8888888", TimeAccuracy.Hundredths, "0:10.88")]
        [InlineData("00:00:11.8888888", TimeAccuracy.Milliseconds, "0:11.888")]
        public void FormatsTimespanCorrectly_WhenDeterminedAccuracyIsGiven(string timespanText, TimeAccuracy accuracy, string expected)
        {
            var formatter = new GeneralTimeFormatter
            {
                DigitsFormat = DigitsFormat.SingleDigitMinutes,
                Accuracy = accuracy
            };
            var time = TimeSpan.Parse(timespanText);
            var formatted = formatter.Format(time);

            Assert.Equal(expected, formatted);
        }
    }
}
