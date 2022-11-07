using LiveSplit.Model;
using System;
using Xunit;

namespace LiveSplit.Tests.TimeParseTests
{
    public class TimeSpanParserTests
    {
        [Theory]
        [InlineData("5:40.41", "00:05:40.4100000")]
        [InlineData("4:20.69", "00:04:20.6900000")]
        [InlineData("04:20.69", "00:04:20.6900000")]
        [InlineData("-12:37:30.12", "-12:37:30.1200000")]
        [InlineData("-37:30.12", "-00:37:30.1200000")]
        [InlineData("-30.12", "-00:00:30.1200000")]
        [InlineData("-10:30", "-00:10:30")]
        [InlineData("-30", "-00:00:30")]
        [InlineData("-100", "-00:01:40")]
        [InlineData("37:03.12", "00:37:03.1200000")]
        [InlineData("10.12345", "00:00:10.1234500")]
        [InlineData("10.1234567", "00:00:10.1234567")]
        [InlineData("10.123456789", "00:00:10.1234567")]
        [InlineData("10.0987654321987654321", "00:00:10.0987654")]
        [InlineData("07:05:1.03", "07:05:01.0300000")]
        [InlineData("00:0:01.00900", "00:00:01.0090000")]
        [InlineData("1:05:01.9999999", "01:05:01.9999999")]

        public void ParsesTimeSpanCorrectly(string timeSpanToParse, string expectedTimeSpan)
        {
            var time = TimeSpanParser.Parse(timeSpanToParse);
            var formattedTime = time.ToString();
            Assert.Equal(expectedTimeSpan, formattedTime);
        }
    }
}
