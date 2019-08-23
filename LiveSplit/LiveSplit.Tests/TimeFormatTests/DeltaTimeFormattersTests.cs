using System;
using LiveSplit.TimeFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveSplit.Tests.TimeFormatterTests
{
    [TestClass]
    public class DeltaTimeFormattersTests 
    {
        // All these formatters (currently) give identical output:
        // - new DeltaTimeFormatter(timeAccuracy, dropDecimals);
        // - new DeltaComponentFormatter(timeAccuracy, dropDecimals);
        // - new DeltaSplitTimeFormatter(timeAccuracy, dropDecimals);
        // - new PreciseDeltaFormatter(timeAccuracy); // dropDecimals is always false

        // note: dash (-) is used for null, and minus (−) for negatives

        [TestMethod]
        [DataRow(null, TimeAccuracy.Hundredths, false, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, false, "+0")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [DataRow("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [DataRow("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [DataRow("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
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
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeAccuracy.Hundredths, false, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, false, "+0")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [DataRow("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [DataRow("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [DataRow("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void TestDeltaComponentFormatter(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expected)
        {
            var formatter = new DeltaComponentFormatter(timeAccuracy, dropDecimals: dropDecimals);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeAccuracy.Hundredths, false, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, false, "+0")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, false, "+1")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, false, "+0.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, false, "+1.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, false, "+1.0")]
        [DataRow("-00:00:01.009", TimeAccuracy.Tenths, false, "−1.0")]
        [DataRow("-00:05:01.999", TimeAccuracy.Tenths, false, "−5:01.9")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, false, "+5:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, false, "+1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, false, "+0.05")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, false, "+227:01:01.99")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, true, "+227:01:01")]
        [DataRow("-9.11:01:01.999", TimeAccuracy.Hundredths, false, "−227:01:01.99")]
        public void TestDeltaSplitTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, bool dropDecimals, string expected) 
        {
            var formatter = new DeltaSplitTimeFormatter(timeAccuracy, dropDecimals: dropDecimals);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeAccuracy.Hundredths, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, "+0")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, "+1")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, "+0.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, "+1.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, "+1.0")]
        [DataRow("-00:00:01.009", TimeAccuracy.Tenths, "−1.0")]
        [DataRow("-00:05:01.999", TimeAccuracy.Tenths, "−5:01.9")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, "+5:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, "+1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, "+0.05")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, "+227:01:01.99")]
        [DataRow("-9.11:01:01.999", TimeAccuracy.Hundredths, "−227:01:01.99")]
        public void TestPreciseDeltaFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new PreciseDeltaFormatter(timeAccuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }
    }
}
