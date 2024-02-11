using System.Collections.Generic;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.TimeParseTests
{
    public class TimeSpanParserTests
    {
        [Theory]
        [MemberData(nameof(TimeSpanFeeder))]
        public void ParsesTimeSpanCorrectly(string timeSpanToParse, string expectedTimeSpan)
        {
            var time = TimeSpanParser.Parse(timeSpanToParse);
            var formattedTime = time.ToString();
            Assert.Equal(expectedTimeSpan, formattedTime);
        }

        public static IEnumerable<object[]> TimeSpanFeeder()
        {
            yield return new object[] { "5:40.41", "00:05:40.4100000" };
            yield return new object[] { "4:20.69", "00:04:20.6900000" };
            yield return new object[] { "04:20.69", "00:04:20.6900000" };
            yield return new object[] { "-12:37:30.12", "-12:37:30.1200000" };
            yield return new object[] { "-37:30.12", "-00:37:30.1200000" };
            yield return new object[] { "-30.12", "-00:00:30.1200000" };
            yield return new object[] { "-10:30", "-00:10:30" };
            yield return new object[] { "-30", "-00:00:30" };
            yield return new object[] { "-100", "-00:01:40" };
            yield return new object[] { "37:03.12", "00:37:03.1200000" };
            yield return new object[] { "10.12345", "00:00:10.1234500" };
            yield return new object[] { "10.1234567", "00:00:10.1234567" };
            yield return new object[] { "10.123456789", "00:00:10.1234567" };
            yield return new object[] { "10.0987654321987654321", "00:00:10.0987654" };
            yield return new object[] { "07:05:1.03", "07:05:01.0300000" };
            yield return new object[] { "00:0:01.00900", "00:00:01.0090000" };
            yield return new object[] { "1:05:01.9999999", "01:05:01.9999999" };
            yield return new object[] { "1.01:57:54", "1.01:57:54" };
        }

        [Theory]
        [MemberData(nameof(TimeSpanFeeder))]
        public void ParsesTimeSpanCorrectly_WhenUsingParseNullable(string timeSpanToParse, string expectedTimeSpan)
        {
            var time = TimeSpanParser.ParseNullable(timeSpanToParse);
            var formattedTime = time.ToString();
            Assert.Equal(expectedTimeSpan, formattedTime);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ReturnsNull_WhenParsingInvalidTimeSpan(string timeSpanToParse)
        {
            var time = TimeSpanParser.ParseNullable(timeSpanToParse);
            Assert.Null(time);
        }
    }
}
