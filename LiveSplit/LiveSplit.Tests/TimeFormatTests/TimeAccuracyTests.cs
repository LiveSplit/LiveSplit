using System;
using LiveSplit.TimeFormatters;
using Xunit;

namespace LiveSplit.Tests.TimeFormatTests
{
    public class TimeAccuracyTests
    {
        [Theory]
        [InlineData("00:00:00", "0:00", TimeAccuracy.Seconds)]
        [InlineData("00:00:01", "0:01.0", TimeAccuracy.Tenths)]
        [InlineData("00:00:02", "0:02.00", TimeAccuracy.Hundredths)]
        [InlineData("00:00:03", "0:03.000", TimeAccuracy.Milliseconds)]

        [InlineData("00:00:04", "0:04", TimeAccuracy.Seconds)]
        [InlineData("00:00:05.2", "0:05.2", TimeAccuracy.Tenths)]
        [InlineData("00:00:06.22", "0:06.22", TimeAccuracy.Hundredths)]
        [InlineData("00:00:07.222", "0:07.222", TimeAccuracy.Milliseconds)]

        [InlineData("00:00:08.8888888", "0:08", TimeAccuracy.Seconds)]
        [InlineData("00:00:09.8888888", "0:09.8", TimeAccuracy.Tenths)]
        [InlineData("00:00:10.8888888", "0:10.88", TimeAccuracy.Hundredths)]
        [InlineData("00:00:11.8888888", "0:11.888", TimeAccuracy.Milliseconds)]
        public void TestTimeAccuracy(string timespanText, string expected, TimeAccuracy accuracy)
        {
            var formatter = new GeneralTimeFormatter();
            formatter.DigitsFormat = DigitsFormat.SingleDigitMinutes;
            formatter.Accuracy = accuracy;

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }
    }
}
