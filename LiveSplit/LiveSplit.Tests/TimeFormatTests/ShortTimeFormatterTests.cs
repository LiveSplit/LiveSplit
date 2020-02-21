using System;
using LiveSplit.TimeFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveSplit.Tests.TimeFormatterTests
{

    [TestClass]
    public class ShortTimeFormatterTests
    {
        // These tests cover the following, which are/were all based on ShortTimeFormatter 
        // new ShortTimeFormatter(); // Format() accepts a TimeFormat
        // new PossibleTimeSaveFormatter(); // ShortTimeFormatter with Accuracy
        // new SegmentTimesFormatter(timeAccuracy); // similar/same as PossibleTimeSaveFormatter

        [TestMethod]
        [DataRow(null, "0.00")]
        [DataRow("00:00:00", "0.00")]
        [DataRow("00:00:01.03", "1.03")]
        [DataRow("00:05:01.03", "5:01.03")]
        [DataRow("-00:05:01.03", "−5:01.03")]
        [DataRow("07:05:01.03", "7:05:01.03")]
        [DataRow("1.07:05:01.03", "31:05:01.03")]

        public void TestShortTimeFormatter(string timespanText, string expected)
        {
            var formatter = new ShortTimeFormatter();

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);

            // test if it's the same as using TimeFormat.Seconds too
            string formatted2 = formatter.Format(time, TimeFormat.Seconds);
            Assert.AreEqual(formatted2, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeFormat.Seconds,  "0.00")] 
        [DataRow(null, TimeFormat.Minutes,  "0.00")]
        [DataRow(null, TimeFormat.Hours,    "0.00")]
        [DataRow(null, TimeFormat.TenHours, "0.00")]

        [DataRow("00:00:00", TimeFormat.Seconds,         "0.00")]
        [DataRow("00:00:00", TimeFormat.Minutes,     "00:00.00")]
        [DataRow("00:00:00", TimeFormat.Hours,     "0:00:00.00")]
        [DataRow("00:00:00", TimeFormat.TenHours, "00:00:00.00")]

        [DataRow("00:00:01.03", TimeFormat.Seconds,         "1.03")]
        [DataRow("00:00:01.03", TimeFormat.Minutes,     "00:01.03")]
        [DataRow("00:00:01.03", TimeFormat.Hours,     "0:00:01.03")]
        [DataRow("00:00:01.03", TimeFormat.TenHours, "00:00:01.03")]

        [DataRow("00:05:01.03", TimeFormat.Seconds,      "5:01.03")]
        [DataRow("00:05:01.03", TimeFormat.Minutes,     "05:01.03")]
        [DataRow("00:05:01.03", TimeFormat.Hours,     "0:05:01.03")]
        [DataRow("00:05:01.03", TimeFormat.TenHours, "00:05:01.03")]

        [DataRow("-00:05:01.03", TimeFormat.Seconds,      "−5:01.03")]
        [DataRow("-00:05:01.03", TimeFormat.Minutes,     "−05:01.03")]
        [DataRow("-00:05:01.03", TimeFormat.Hours,     "−0:05:01.03")]
        [DataRow("-00:05:01.03", TimeFormat.TenHours, "−00:05:01.03")]

        [DataRow("07:05:01.03", TimeFormat.Seconds,   "7:05:01.03")]
        [DataRow("07:05:01.03", TimeFormat.Minutes,   "7:05:01.03")]
        [DataRow("07:05:01.03", TimeFormat.Hours,     "7:05:01.03")]
        [DataRow("07:05:01.03", TimeFormat.TenHours, "07:05:01.03")]

        [DataRow("1.07:05:01.03", TimeFormat.Seconds,  "31:05:01.03")]
        [DataRow("1.07:05:01.03", TimeFormat.Minutes,  "31:05:01.03")]
        [DataRow("1.07:05:01.03", TimeFormat.Hours,    "31:05:01.03")]
        [DataRow("1.07:05:01.03", TimeFormat.TenHours, "31:05:01.03")]
        public void TestShortTimeFormatterWithTimeFormat(string timespanText, TimeFormat format, string expected)
        {
            var formatter = new ShortTimeFormatter();

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time, format);
            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        [DataRow(null, TimeAccuracy.Seconds, "-")] 
        [DataRow(null, TimeAccuracy.Tenths, "-")]
        [DataRow(null, TimeAccuracy.Hundredths, "-")] 

        [DataRow("00:00:00", TimeAccuracy.Seconds, "0")]
        [DataRow("00:00:00", TimeAccuracy.Tenths, "0.0")]
        [DataRow("00:00:00", TimeAccuracy.Hundredths, "0.00")]
        
        [DataRow("00:05:01", TimeAccuracy.Seconds, "5:01")]
        [DataRow("00:05:01.2", TimeAccuracy.Seconds, "5:01")]
        [DataRow("00:05:01.02", TimeAccuracy.Seconds, "5:01")]
        [DataRow("00:05:01.002", TimeAccuracy.Seconds, "5:01")]

        [DataRow("00:05:01", TimeAccuracy.Tenths, "5:01.0")]
        [DataRow("00:05:01.2", TimeAccuracy.Tenths, "5:01.2")]
        [DataRow("00:05:01.02", TimeAccuracy.Tenths, "5:01.0")]
        [DataRow("00:05:01.002", TimeAccuracy.Tenths, "5:01.0")]

        [DataRow("00:05:01", TimeAccuracy.Hundredths, "5:01.00")]
        [DataRow("00:05:01.2", TimeAccuracy.Hundredths, "5:01.20")]
        [DataRow("00:05:01.02", TimeAccuracy.Hundredths, "5:01.02")]
        [DataRow("00:05:01.002", TimeAccuracy.Hundredths, "5:01.00")]

        [DataRow("-00:05:01.03", TimeAccuracy.Seconds, "−5:01")]
        [DataRow("-00:05:01.03", TimeAccuracy.Tenths, "−5:01.0")]
        [DataRow("-00:05:01.03", TimeAccuracy.Hundredths, "−5:01.03")]

        [DataRow("07:05:01.29", TimeAccuracy.Seconds, "7:05:01")]
        [DataRow("07:05:01.29", TimeAccuracy.Tenths, "7:05:01.2")]
        [DataRow("07:05:01.29", TimeAccuracy.Hundredths, "7:05:01.29")]

        [DataRow("1.07:05:01.9999", TimeAccuracy.Seconds, "31:05:01")]
        [DataRow("1.07:05:01.9999", TimeAccuracy.Tenths, "31:05:01.9")]
        [DataRow("1.07:05:01.9999", TimeAccuracy.Hundredths, "31:05:01.99")]
        public void TestPossibleTimeSaveFormatter(string timespanText, TimeAccuracy accuracy, string expected)
        {
            var formatter = new PossibleTimeSaveFormatter();
            formatter.Accuracy = accuracy;

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

        [DataRow("00:00:00", TimeAccuracy.Seconds, "0")]
        [DataRow("00:00:00", TimeAccuracy.Tenths, "0.0")]
        [DataRow("00:00:00", TimeAccuracy.Hundredths, "0.00")]

        [DataRow("00:05:01", TimeAccuracy.Seconds, "5:01")]
        [DataRow("00:05:01.2", TimeAccuracy.Seconds, "5:01")]
        [DataRow("00:05:01.02", TimeAccuracy.Seconds, "5:01")]
        [DataRow("00:05:01.002", TimeAccuracy.Seconds, "5:01")]

        [DataRow("00:05:01", TimeAccuracy.Tenths, "5:01.0")]
        [DataRow("00:05:01.2", TimeAccuracy.Tenths, "5:01.2")]
        [DataRow("00:05:01.02", TimeAccuracy.Tenths, "5:01.0")]
        [DataRow("00:05:01.002", TimeAccuracy.Tenths, "5:01.0")]

        [DataRow("00:05:01", TimeAccuracy.Hundredths, "5:01.00")]
        [DataRow("00:05:01.2", TimeAccuracy.Hundredths, "5:01.20")]
        [DataRow("00:05:01.02", TimeAccuracy.Hundredths, "5:01.02")]
        [DataRow("00:05:01.002", TimeAccuracy.Hundredths, "5:01.00")]

        [DataRow("-00:05:01.03", TimeAccuracy.Seconds, "−5:01")]
        [DataRow("-00:05:01.03", TimeAccuracy.Tenths, "−5:01.0")]
        [DataRow("-00:05:01.03", TimeAccuracy.Hundredths, "−5:01.03")]

        [DataRow("07:05:01.29", TimeAccuracy.Seconds, "7:05:01")]
        [DataRow("07:05:01.29", TimeAccuracy.Tenths, "7:05:01.2")]
        [DataRow("07:05:01.29", TimeAccuracy.Hundredths, "7:05:01.29")]

        [DataRow("1.07:05:01.9999", TimeAccuracy.Seconds, "31:05:01")]
        [DataRow("1.07:05:01.9999", TimeAccuracy.Tenths, "31:05:01.9")]
        [DataRow("1.07:05:01.9999", TimeAccuracy.Hundredths, "31:05:01.99")]
        public void TestSegmentTimesFormatter(string timespanText, TimeAccuracy accuracy, string expected)
        {
            var formatter = new SegmentTimesFormatter(accuracy);

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }
    }
}