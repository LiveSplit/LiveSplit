using System;
using LiveSplit.TimeFormatters;
using Xunit;

namespace LiveSplit.Tests.TimeFormatterTests
{
    public class DeltaTimeFormattersTests 
    {
        // All these formatters (currently) give identical output:
        // - new DeltaTimeFormatter(timeAccuracy, dropDecimals);
        // - new DeltaComponentFormatter(timeAccuracy, dropDecimals);
        // - new DeltaSplitTimeFormatter(timeAccuracy, dropDecimals);
        // - new PreciseDeltaFormatter(timeAccuracy); // dropDecimals is always false

        // note: dash (-) is used for null, and minus (−) for negatives

        [Fact]
        public void DeltaTimeFormatterFormatsTimeAsDash_WhenTimeIsNullAndAccuracyIsInHundredths()
        {
            var sut = new DeltaTimeFormatter
            {
                Accuracy = TimeAccuracy.Hundredths,
                DropDecimals = false
            };

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }
        
        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "0")]
        [InlineData("00:00:00.001", TimeAccuracy.Seconds, false, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, false, "0.0")]
        [InlineData("00:00:00.001", TimeAccuracy.Tenths, false, "+0.0")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, false, "0.00")]
        [InlineData("00:00:00.001", TimeAccuracy.Hundredths, false, "+0.00")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void DeltaTimeFormatterFormatsTimeInGivenAccuracy_WhenTimeIsValid(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expectedDelta)
        {
            var sut = new DeltaTimeFormatter
            {
                Accuracy = timeAccuracy,
                DropDecimals = dropDecimals
            };

            var time = TimeSpan.Parse(timespanText);

            var formattedDelta = sut.Format(time);
            Assert.Equal(expectedDelta, formattedDelta);
        }

        [Fact]
        public void DeltaComponentFormatterFormatsTimeAsDash_WhenTimeIsNullAndAccuracyIsInHundredths()
        {
            var sut = new DeltaComponentFormatter(TimeAccuracy.Hundredths, false);

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }

        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "0")]
        [InlineData("00:00:00.001", TimeAccuracy.Seconds, false, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, false, "0.0")]
        [InlineData("00:00:00.001", TimeAccuracy.Tenths, false, "+0.0")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, false, "0.00")]
        [InlineData("00:00:00.001", TimeAccuracy.Hundredths, false, "+0.00")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void DeltaComponentFormatterFormatsTimeCorrectly_WhenTimeIsValid(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expectedDelta)
        {
            var sut = new DeltaComponentFormatter(timeAccuracy, dropDecimals: dropDecimals);
            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedDelta, formattedTime);
        }

        [Fact]
        public void DeltaSplitTimeFormatterFormatsTimeAsDash_WhenTimeIsNull()
        {
            var sut = new DeltaSplitTimeFormatter(TimeAccuracy.Hundredths, dropDecimals: false);

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }
        
        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "0")]
        [InlineData("00:00:00.001", TimeAccuracy.Seconds, false, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, false, "0.0")]
        [InlineData("00:00:00.001", TimeAccuracy.Tenths, false, "+0.0")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, false, "0.00")]
        [InlineData("00:00:00.001", TimeAccuracy.Hundredths, false, "+0.00")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void DeltaSplitTimeFormatterFormatsTimeCorrectly_WhenTimeIsValid(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expectedDelta) 
        {
            var sut = new DeltaSplitTimeFormatter(timeAccuracy, dropDecimals: dropDecimals);
            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedDelta, formattedTime);
        }
    }
}