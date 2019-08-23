using System;
using LiveSplit.TimeFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveSplit.Tests.TimeFormatterTests 
{

    [TestClass]
    public class DaysTimeFormatterTests
    {
        // These tests cover DaysTimeFormatter, which (is/was not) based on any other TimeFormatter implementation

        [TestMethod]
        [DataRow(null, "0")]
        [DataRow("00:00:00", "0:00")]
        [DataRow("00:00:01.03", "0:01")]
        [DataRow("00:05:01.03", "5:01")]
        [DataRow("07:05:01.03", "7:05:01")]
        [DataRow("1.07:05:01.03", "1d 7:05:01")]
        [DataRow("9.23:02:03.412", "9d 23:02:03")]
        [DataRow("272.13:04:05.612", "272d 13:04:05")]
        public void TestDaysTimeFormatter(string timespanText, string expected)
        {
            var formatter = new DaysTimeFormatter();

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }

        /*
        // These tests will fail until changes are made.
        // Currently:
        // - negative TimeSpans are displayed as positive
        // - negative days + hours are not displayed
        [TestMethod]
        [DataRow("-00:05:01.03", "−5:01")] // Actual:<5:01> [Fail]
        [DataRow("-1.00:00:01.999", "−1d 0:00:01")] // Actual:<0:01> [Fail]
        [DataRow("-9.23:02:03.412", "−9d 23:02:03")] // Actual:<2:03>. [Fail]
        [DataRow("-222.13:04:05.612", "−222d 13:04:05")] // Actual:<4:05>. [Fail]
        public void NegativeTestDaysTimeFormatter(string timespanText, string expected) {
        var formatter = new DaysTimeFormatter();

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }
        */

    }
}
