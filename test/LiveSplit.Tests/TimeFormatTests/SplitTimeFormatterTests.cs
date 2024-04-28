using System;
using LiveSplit.TimeFormatters;
using Xunit;

namespace LiveSplit.Tests.TimeFormatterTests
{
    public class SplitTimeFormattersTests
    {   
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
        public void SplitTimeFormatterFormatsTimeCorrectlyInGivenAccuracy_WhenTimeIsValid(string timespanText, TimeAccuracy timeAccuracy, string expectedTime)
        {
            var sut = new SplitTimeFormatter(timeAccuracy);
            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedTime, formattedTime);
        }

        [Theory]
        [InlineData(TimeAccuracy.Seconds)]
        [InlineData(TimeAccuracy.Tenths)]
        [InlineData(TimeAccuracy.Hundredths)]
        public void SplitTimeFormatterFormatsTimeAsDash_WhenTimeIsNullRegardlessOfAccuracy(TimeAccuracy timeAccuracy)
        {
            var sut = new SplitTimeFormatter(timeAccuracy);

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }
    }
}
