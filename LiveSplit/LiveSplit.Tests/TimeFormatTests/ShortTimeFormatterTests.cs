using System;
using Xunit;
using LiveSplit.TimeFormatters;

namespace LiveSplit.Tests.TimeFormatterTests
{
    public class ShortTimeFormatterTests
    {
        [Fact]
        public void FormatsTimeCorrectly_WhenTimeIsNullAndNoFormatIsSupplied()
        {
            var sut = new ShortTimeFormatter();

            var formattedTime = sut.Format(null);
            Assert.Equal("0.00", formattedTime);
        }

        [Fact]
        public void FormatsTimeInSecondsByDefault_WhenTimeIsNullAndNoFormatIsSupplied()
        {
            var sut = new ShortTimeFormatter();
            var defaultFormat = sut.Format(null);

            var formattedValueInSeconds = sut.Format(null, TimeFormat.Seconds);
            Assert.Equal(defaultFormat, formattedValueInSeconds);
        }

        [Theory]
        [InlineData("00:00:00", "0.00")]
        [InlineData("00:00:01.03", "1.03")]
        [InlineData("00:05:01.03", "5:01.03")]
        [InlineData("-00:05:01.03", "−5:01.03")]
        [InlineData("07:05:01.03", "7:05:01.03")]
        [InlineData("1.07:05:01.03", "31:05:01.03")]
        public void FormatsTimeCorrectly_WhenTimeIsValidAndNoFormatIsSupplied(string timespanText, string expectedTime)
        {
            var sut = new ShortTimeFormatter();
            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedTime, formattedTime);
        }

        [Theory]
        [InlineData("00:00:00")]
        [InlineData("00:00:01.03")]
        [InlineData("00:05:01.03")]
        [InlineData("-00:05:01.03")]
        [InlineData("07:05:01.03")]
        [InlineData("1.07:05:01.03")]
        public void FormatsTimeInSecondsByDefault_WhenTimeIsValidAndNoFormatIsSupplied(string timespanText)
        {
            var sut = new ShortTimeFormatter();
            var time = TimeSpan.Parse(timespanText);

            var defaultFormat = sut.Format(time);
            var formattedTimeInSeconds = sut.Format(time, TimeFormat.Seconds);
            Assert.Equal(defaultFormat, formattedTimeInSeconds);
        }

        [Theory]
        [InlineData(TimeFormat.Seconds)]
        [InlineData(TimeFormat.Minutes)]
        [InlineData(TimeFormat.Hours)]
        [InlineData(TimeFormat.TenHours)]
        public void FormatsNullTimeInGivenFormatCorrectly(TimeFormat givenFormat)
        {
            var sut = new ShortTimeFormatter();

            var formattedTime = sut.Format(null, givenFormat);
            Assert.Equal("0.00", formattedTime);
        }

        [Theory]
        [InlineData("00:00:00", TimeFormat.Seconds, "0.00")]
        [InlineData("00:00:00", TimeFormat.Minutes, "00:00.00")]
        [InlineData("00:00:00", TimeFormat.Hours, "0:00:00.00")]
        [InlineData("00:00:00", TimeFormat.TenHours, "00:00:00.00")]

        [InlineData("00:00:01.03", TimeFormat.Seconds, "1.03")]
        [InlineData("00:00:01.03", TimeFormat.Minutes, "00:01.03")]
        [InlineData("00:00:01.03", TimeFormat.Hours, "0:00:01.03")]
        [InlineData("00:00:01.03", TimeFormat.TenHours, "00:00:01.03")]

        [InlineData("00:05:01.03", TimeFormat.Seconds, "5:01.03")]
        [InlineData("00:05:01.03", TimeFormat.Minutes, "05:01.03")]
        [InlineData("00:05:01.03", TimeFormat.Hours, "0:05:01.03")]
        [InlineData("00:05:01.03", TimeFormat.TenHours, "00:05:01.03")]

        [InlineData("-00:05:01.03", TimeFormat.Seconds, "−5:01.03")]
        [InlineData("-00:05:01.03", TimeFormat.Minutes, "−05:01.03")]
        [InlineData("-00:05:01.03", TimeFormat.Hours, "−0:05:01.03")]
        [InlineData("-00:05:01.03", TimeFormat.TenHours, "−00:05:01.03")]

        [InlineData("07:05:01.03", TimeFormat.Seconds, "7:05:01.03")]
        [InlineData("07:05:01.03", TimeFormat.Minutes, "7:05:01.03")]
        [InlineData("07:05:01.03", TimeFormat.Hours, "7:05:01.03")]
        [InlineData("07:05:01.03", TimeFormat.TenHours, "07:05:01.03")]

        [InlineData("1.07:05:01.03", TimeFormat.Seconds, "31:05:01.03")]
        [InlineData("1.07:05:01.03", TimeFormat.Minutes, "31:05:01.03")]
        [InlineData("1.07:05:01.03", TimeFormat.Hours, "31:05:01.03")]
        [InlineData("1.07:05:01.03", TimeFormat.TenHours, "31:05:01.03")]
        public void FormatsTimeCorrectly_WhenTimeIsValidAndTimeFormatIsSupplied(string timespanText, TimeFormat format,
            string expectedTime)
        {
            var sut = new ShortTimeFormatter();
            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time, format);
            Assert.Equal(expectedTime, formattedTime);
        }
    }

    public class SegmentTimesFormatterShould
    {
        [Theory]
        [InlineData(TimeAccuracy.Seconds)]
        [InlineData(TimeAccuracy.Tenths)]
        [InlineData(TimeAccuracy.Hundredths)]
        public void FormatTimeAsDash_WhenTimeIsNullRegardlessOfAccuracy(TimeAccuracy accuracy)
        {
            var sut = new SegmentTimesFormatter(accuracy);

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }

        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, "0")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, "0.0")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, "0.00")]

        [InlineData("00:05:01", TimeAccuracy.Seconds, "5:01")]
        [InlineData("00:05:01.2", TimeAccuracy.Seconds, "5:01")]
        [InlineData("00:05:01.02", TimeAccuracy.Seconds, "5:01")]
        [InlineData("00:05:01.002", TimeAccuracy.Seconds, "5:01")]

        [InlineData("00:05:01", TimeAccuracy.Tenths, "5:01.0")]
        [InlineData("00:05:01.2", TimeAccuracy.Tenths, "5:01.2")]
        [InlineData("00:05:01.02", TimeAccuracy.Tenths, "5:01.0")]
        [InlineData("00:05:01.002", TimeAccuracy.Tenths, "5:01.0")]

        [InlineData("00:05:01", TimeAccuracy.Hundredths, "5:01.00")]
        [InlineData("00:05:01.2", TimeAccuracy.Hundredths, "5:01.20")]
        [InlineData("00:05:01.02", TimeAccuracy.Hundredths, "5:01.02")]
        [InlineData("00:05:01.002", TimeAccuracy.Hundredths, "5:01.00")]

        [InlineData("-00:05:01.03", TimeAccuracy.Seconds, "−5:01")]
        [InlineData("-00:05:01.03", TimeAccuracy.Tenths, "−5:01.0")]
        [InlineData("-00:05:01.03", TimeAccuracy.Hundredths, "−5:01.03")]

        [InlineData("07:05:01.29", TimeAccuracy.Seconds, "7:05:01")]
        [InlineData("07:05:01.29", TimeAccuracy.Tenths, "7:05:01.2")]
        [InlineData("07:05:01.29", TimeAccuracy.Hundredths, "7:05:01.29")]

        [InlineData("1.07:05:01.9999", TimeAccuracy.Seconds, "31:05:01")]
        [InlineData("1.07:05:01.9999", TimeAccuracy.Tenths, "31:05:01.9")]
        [InlineData("1.07:05:01.9999", TimeAccuracy.Hundredths, "31:05:01.99")]
        public void FormatTimeCorrectly_WhenTimeIsValidAndAccuracyIsSupplied(string timespanText, TimeAccuracy accuracy, string expectedTime)
        {
            var sut = new SegmentTimesFormatter(accuracy);
            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedTime, formattedTime);
        }
    }

    public class PossibleTimeSaveFormatterShould
    {
        [Theory]
        [InlineData(TimeAccuracy.Seconds)]
        [InlineData(TimeAccuracy.Tenths)]
        [InlineData(TimeAccuracy.Hundredths)]
        public void FormatTimeAsDash_WhenTimeIsNullRegardlessOfAccuracy(TimeAccuracy accuracy)
        {
            var sut = new PossibleTimeSaveFormatter
            {
                Accuracy = accuracy
            };

            var formattedTime = sut.Format(null);
            Assert.Equal(TimeFormatConstants.DASH, formattedTime);
        }

        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, "0")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, "0.0")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, "0.00")]

        [InlineData("00:05:01", TimeAccuracy.Seconds, "5:01")]
        [InlineData("00:05:01.2", TimeAccuracy.Seconds, "5:01")]
        [InlineData("00:05:01.02", TimeAccuracy.Seconds, "5:01")]
        [InlineData("00:05:01.002", TimeAccuracy.Seconds, "5:01")]

        [InlineData("00:05:01", TimeAccuracy.Tenths, "5:01.0")]
        [InlineData("00:05:01.2", TimeAccuracy.Tenths, "5:01.2")]
        [InlineData("00:05:01.02", TimeAccuracy.Tenths, "5:01.0")]
        [InlineData("00:05:01.002", TimeAccuracy.Tenths, "5:01.0")]

        [InlineData("00:05:01", TimeAccuracy.Hundredths, "5:01.00")]
        [InlineData("00:05:01.2", TimeAccuracy.Hundredths, "5:01.20")]
        [InlineData("00:05:01.02", TimeAccuracy.Hundredths, "5:01.02")]
        [InlineData("00:05:01.002", TimeAccuracy.Hundredths, "5:01.00")]

        [InlineData("-00:05:01.03", TimeAccuracy.Seconds, "−5:01")]
        [InlineData("-00:05:01.03", TimeAccuracy.Tenths, "−5:01.0")]
        [InlineData("-00:05:01.03", TimeAccuracy.Hundredths, "−5:01.03")]

        [InlineData("07:05:01.29", TimeAccuracy.Seconds, "7:05:01")]
        [InlineData("07:05:01.29", TimeAccuracy.Tenths, "7:05:01.2")]
        [InlineData("07:05:01.29", TimeAccuracy.Hundredths, "7:05:01.29")]

        [InlineData("1.07:05:01.9999", TimeAccuracy.Seconds, "31:05:01")]
        [InlineData("1.07:05:01.9999", TimeAccuracy.Tenths, "31:05:01.9")]
        [InlineData("1.07:05:01.9999", TimeAccuracy.Hundredths, "31:05:01.99")]
        public void FormatTimeCorrectly_WhenTimeIsValidAndAccuracyIsSupplied(string timespanText, TimeAccuracy accuracy,
            string expectedTime)
        {
            var sut = new PossibleTimeSaveFormatter
            {
                Accuracy = accuracy
            };

            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedTime, formattedTime);
        }

        [Theory]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "0")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, false, "0.0")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, false, "0.00")]

        [InlineData("00:00:00", TimeAccuracy.Seconds, true, "0")]
        [InlineData("00:00:00", TimeAccuracy.Tenths, true, "0.0")]
        [InlineData("00:00:00", TimeAccuracy.Hundredths, true, "0.00")]

        [InlineData("00:01:30.00", TimeAccuracy.Seconds, false, "1:30")]
        [InlineData("00:01:30.00", TimeAccuracy.Tenths, false, "1:30.0")]
        [InlineData("00:01:30.00", TimeAccuracy.Hundredths, false, "1:30.00")]

        [InlineData("00:01:30.00", TimeAccuracy.Seconds, true, "1:30")]
        [InlineData("00:01:30.00", TimeAccuracy.Tenths, true, "1:30")]
        [InlineData("00:01:30.00", TimeAccuracy.Hundredths, true, "1:30")]

        [InlineData("07:05:01.29", TimeAccuracy.Seconds, false, "7:05:01")]
        [InlineData("07:05:01.29", TimeAccuracy.Tenths, false, "7:05:01.2")]
        [InlineData("07:05:01.29", TimeAccuracy.Hundredths, false, "7:05:01.29")]

        [InlineData("07:05:01.29", TimeAccuracy.Seconds, true, "7:05:01")]
        [InlineData("07:05:01.29", TimeAccuracy.Tenths, true, "7:05:01")]
        [InlineData("07:05:01.29", TimeAccuracy.Hundredths, true, "7:05:01")]
        public void FormatTimeCorrectly_WhenDecimalsDropped(string timespanText, TimeAccuracy accuracy,
            bool dropDecimals, string expectedTime)
        {
            var sut = new PossibleTimeSaveFormatter
            {
                DropDecimals = dropDecimals,
                Accuracy = accuracy
            };

            var time = TimeSpan.Parse(timespanText);

            var formattedTime = sut.Format(time);
            Assert.Equal(expectedTime, formattedTime);
        }
    }
}