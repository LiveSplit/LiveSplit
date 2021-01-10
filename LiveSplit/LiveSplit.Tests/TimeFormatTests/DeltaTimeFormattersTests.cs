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

        [Theory]
        [InlineData(null, TimeAccuracy.Hundredths, false, "-")]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void TestDeltaTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expected)
        {
            var formatter = new DeltaTimeFormatter {
                Accuracy = timeAccuracy,
                DropDecimals = dropDecimals
            };

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }

        [Theory]
        [InlineData(null, TimeAccuracy.Hundredths, false, "-")]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void TestDeltaComponentFormatter(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expected)
        {
            var formatter = new DeltaComponentFormatter(timeAccuracy, dropDecimals: dropDecimals);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }

        [Theory]
        [InlineData(null, TimeAccuracy.Hundredths, false, "-")]
        [InlineData("00:00:00", TimeAccuracy.Seconds, false, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void TestDeltaSplitTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expected) 
        {
            var formatter = new DeltaSplitTimeFormatter(timeAccuracy, dropDecimals: dropDecimals);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }

        [Theory]
        [InlineData(null, TimeAccuracy.Hundredths, "-")]
        [InlineData("00:00:00", TimeAccuracy.Seconds, "+0")]
        [InlineData("00:00:01", TimeAccuracy.Seconds, "+1")]
        [InlineData("00:00:00.5", TimeAccuracy.Tenths, "+0.5")]
        [InlineData("00:00:01.001", TimeAccuracy.Tenths, "+1.0")]
        [InlineData("00:00:01.009", TimeAccuracy.Tenths, "+1.0")]
        [InlineData("-00:00:01.009", TimeAccuracy.Tenths, "−1.0")]
        [InlineData("-00:05:01.999", TimeAccuracy.Tenths, "−5:01.9")]
        [InlineData("00:05:01.999", TimeAccuracy.Tenths, "+5:01.9")]
        [InlineData("01:05:01.999", TimeAccuracy.Tenths, "+1:05:01.9")]
        [InlineData("00:00:00.05", TimeAccuracy.Hundredths, "+0.05")]
        [InlineData("9.11:01:01.999", TimeAccuracy.Hundredths, "+227:01:01.99")]
        [InlineData("-9.11:01:01.999", TimeAccuracy.Hundredths, "−227:01:01.99")]
        public void TestPreciseDeltaFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new PreciseDeltaFormatter(timeAccuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.Equal(expected, formatted);
        }
    }
}
