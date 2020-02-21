using System;
using LiveSplit.TimeFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveSplit.Tests.TimeFormatTests
{
    [TestClass]
    public class ShowDaysTests
    {
        [TestMethod]
        [DataRow("00:00:01", "1", DigitsFormat.SingleDigitSeconds)]
        [DataRow("00:00:02", "02", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("00:00:03", "0:03", DigitsFormat.SingleDigitMinutes)]
        [DataRow("00:00:04", "00:04", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("00:00:05", "0:00:05", DigitsFormat.SingleDigitHours)]
        [DataRow("00:00:06", "00:00:06", DigitsFormat.DoubleDigitHours)]

        [DataRow("1:02:51:12", "1d 2:51:12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("1:03:42:34", "1d 3:42:34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("1:04:33:56", "1d 4:33:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("1:05:24:23", "1d 5:24:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("1:01:15:45", "1d 1:15:45", DigitsFormat.SingleDigitHours)]
        [DataRow("1:02:56:51", "1d 02:56:51", DigitsFormat.DoubleDigitHours)]

        [DataRow("1:00:00:00", "1d 0:00:00", DigitsFormat.SingleDigitHours)]
        [DataRow("3:00:00:00", "3d 00:00:00", DigitsFormat.DoubleDigitHours)]

        [DataRow("2:00:00:23", "2d 0:00:23", DigitsFormat.SingleDigitHours)]
        [DataRow("4:00:00:23", "4d 00:00:23", DigitsFormat.DoubleDigitHours)]

        [DataRow("-5:01:15:45", "−5d 1:15:45", DigitsFormat.SingleDigitHours)]
        [DataRow("-6:02:56:51", "−6d 02:56:51", DigitsFormat.DoubleDigitHours)]
        public void TestShowDays(string timespanText, string expected, DigitsFormat format)
        {
            var formatter = new GeneralTimeFormatter();
            formatter.DigitsFormat = format;
            formatter.ShowDays = true;
            formatter.Accuracy = TimeAccuracy.Seconds;

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }
    }
}
