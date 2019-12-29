using System;
using LiveSplit.TimeFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveSplit.Tests.TimeFormatterTests
{

    [TestClass]
    public class RegularTimeFormattersTests
    {
        // These tests cover the following, which are/were all based on RegularTimeFormatter:
        //new RegularTimeFormatter(timeAccuracy);
        //new RegularSplitTimeFormatter(timeAccuracy); // dash for null values
        //new RunPredictionFormatter(timeAccuracy); // dash for null values
        //new RegularSumOfBestTimeFormatter(); // (Accuracy can be set but not in constructor)
        //new AutomaticPrecisionTimeFormatter(); // -> RegularTimeFormatter with changes

        [TestMethod]
        [DataRow(null, TimeAccuracy.Seconds, "0")]
        [DataRow(null, TimeAccuracy.Tenths, "0.0")]
        [DataRow(null, TimeAccuracy.Hundredths, "0.00")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, "0:00")]
        [DataRow("00:00:00", TimeAccuracy.Tenths, "0:00.0")]
        [DataRow("00:00:00", TimeAccuracy.Hundredths, "0:00.00")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, "0:01")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, "0:00.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, "5:01.9")]
        [DataRow("00:25:01.999", TimeAccuracy.Tenths, "25:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, "1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, "0:00.05")]
        [DataRow("00:10:00.006", TimeAccuracy.Hundredths, "10:00.00")]
        [DataRow("9.11:01:01.999", TimeAccuracy.Hundredths, "227:01:01.99")]
        [DataRow("1.00:00:01.999", TimeAccuracy.Hundredths, "24:00:01.99")] 

        public void TestRegularTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new RegularTimeFormatter(timeAccuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeAccuracy.Seconds, "-")]
        [DataRow(null, TimeAccuracy.Tenths, "-")]
        [DataRow(null, TimeAccuracy.Hundredths, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, "0:00")]
        [DataRow("00:00:00", TimeAccuracy.Tenths, "0:00.0")]
        [DataRow("00:00:00", TimeAccuracy.Hundredths, "0:00.00")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, "0:01")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, "0:00.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, "5:01.9")]
        [DataRow("00:25:01.999", TimeAccuracy.Tenths, "25:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, "1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, "0:00.05")]
        [DataRow("00:10:00.006", TimeAccuracy.Hundredths, "10:00.00")]
        public void TestRegularSplitTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new RegularSplitTimeFormatter(timeAccuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }


        [TestMethod]
        [DataRow(null, TimeAccuracy.Seconds, "-")]
        [DataRow(null, TimeAccuracy.Tenths, "-")]
        [DataRow(null, TimeAccuracy.Hundredths, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, "0:00")]
        [DataRow("00:00:00", TimeAccuracy.Tenths, "0:00.0")]
        [DataRow("00:00:00", TimeAccuracy.Hundredths, "0:00.00")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, "0:01")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, "0:00.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, "5:01.9")]
        [DataRow("00:25:01.999", TimeAccuracy.Tenths, "25:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, "1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, "0:00.05")]
        [DataRow("00:10:00.006", TimeAccuracy.Hundredths, "10:00.00")]
        public void TestRunPredictionFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new RunPredictionFormatter(timeAccuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeAccuracy.Seconds, "-")]
        [DataRow(null, TimeAccuracy.Tenths, "-")]
        [DataRow(null, TimeAccuracy.Hundredths, "-")]
        [DataRow("00:00:00", TimeAccuracy.Seconds, "0:00")]
        [DataRow("00:00:00", TimeAccuracy.Tenths, "0:00.0")]
        [DataRow("00:00:00", TimeAccuracy.Hundredths, "0:00.00")]
        [DataRow("00:00:01", TimeAccuracy.Seconds, "0:01")]
        [DataRow("00:00:00.5", TimeAccuracy.Tenths, "0:00.5")]
        [DataRow("00:00:01.001", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:00:01.009", TimeAccuracy.Tenths, "0:01.0")]
        [DataRow("00:05:01.999", TimeAccuracy.Tenths, "5:01.9")]
        [DataRow("00:25:01.999", TimeAccuracy.Tenths, "25:01.9")]
        [DataRow("01:05:01.999", TimeAccuracy.Tenths, "1:05:01.9")]
        [DataRow("00:00:00.05", TimeAccuracy.Hundredths, "0:00.05")]
        [DataRow("00:10:00.006", TimeAccuracy.Hundredths, "10:00.00")]
        public void TestRegularSumOfBestTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new RegularSumOfBestTimeFormatter();
            formatter.Accuracy = timeAccuracy;

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, "0")]
        [DataRow("00:00:00", "0:00")]
        [DataRow("00:00:01", "0:01")]
        [DataRow("00:00:02.0", "0:02")]
        [DataRow("00:00:03.00", "0:03")]
        [DataRow("00:00:01.2", "0:01.2")]
        [DataRow("00:00:01.20", "0:01.2")]
        [DataRow("00:00:01.200", "0:01.2")]
        [DataRow("00:00:01.23", "0:01.23")]
        [DataRow("00:00:01.234", "0:01.23")]
        [DataRow("00:00:01.2345", "0:01.23")]
        [DataRow("00:00:01.2000", "0:01.2")]
        [DataRow("00:00:01.204", "0:01.20")]
        [DataRow("00:00:01.2005", "0:01.20")]
        [DataRow("00:05:01.999", "5:01.99")]
        [DataRow("00:25:01.999", "25:01.99")]
        [DataRow("00:10:00.006", "10:00.00")]
        public void TestAutomaticPrecisionTimeFormatter(string timespanText, string expected)
        {
            var formatter = new AutomaticPrecisionTimeFormatter();

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        
        [TestMethod]
        [DataRow("-00:00:00.05", TimeAccuracy.Hundredths, "−0:00.05")] // Actual:<0:00.05> [Fail]
        [DataRow("-00:00:01.009", TimeAccuracy.Tenths, "−0:01.0")] // Actual:<0:01.0> [Fail]
        [DataRow("-00:05:01.999", TimeAccuracy.Tenths, "−5:01.9")] // Actual:<5:01.9> [Fail]
        [DataRow("-9.12:09:01.999", TimeAccuracy.Hundredths, "−228:09:01.99")] // Actual:<9:01.99> [Fail]
        [DataRow("-1.00:02:01.999", TimeAccuracy.Hundredths, "−24:02:01.99")] // Actual:<0:01.99> [Fail]
        public void NegativesTestRegularTimeFormatter(string timespanText, TimeAccuracy timeAccuracy, string expected)
        {
            var formatter = new RegularTimeFormatter(timeAccuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }
    }
}
