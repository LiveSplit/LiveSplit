using System;
using LiveSplit.TimeFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveSplit.Tests.TimeFormatTests
{
    [TestClass]
    public class DigitsFormatTests
    {
        [TestMethod]
        [DataRow("00:00:00", "0", DigitsFormat.SingleDigitSeconds)]
        [DataRow("00:00:00", "00", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("00:00:00", "0:00", DigitsFormat.SingleDigitMinutes)]
        [DataRow("00:00:00", "00:00", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("00:00:00", "0:00:00", DigitsFormat.SingleDigitHours)]
        [DataRow("00:00:00", "00:00:00", DigitsFormat.DoubleDigitHours)]

        [DataRow("00:00:01", "1", DigitsFormat.SingleDigitSeconds)]
        [DataRow("00:00:02", "02", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("00:00:03", "0:03", DigitsFormat.SingleDigitMinutes)]
        [DataRow("00:00:04", "00:04", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("00:00:05", "0:00:05", DigitsFormat.SingleDigitHours)]
        [DataRow("00:00:06", "00:00:06", DigitsFormat.DoubleDigitHours)]

        [DataRow("00:00:12", "12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("00:00:34", "34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("00:00:56", "0:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("00:00:23", "00:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("00:00:45", "0:00:45", DigitsFormat.SingleDigitHours)]
        [DataRow("00:00:51", "00:00:51", DigitsFormat.DoubleDigitHours)]

        [DataRow("00:01:12", "1:12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("00:02:34", "2:34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("00:03:56", "3:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("00:04:23", "04:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("00:05:45", "0:05:45", DigitsFormat.SingleDigitHours)]
        [DataRow("00:06:51", "00:06:51", DigitsFormat.DoubleDigitHours)]

        [DataRow("00:51:12", "51:12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("00:42:34", "42:34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("00:33:56", "33:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("00:24:23", "24:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("00:15:45", "0:15:45", DigitsFormat.SingleDigitHours)]
        [DataRow("00:56:51", "00:56:51", DigitsFormat.DoubleDigitHours)]

        [DataRow("02:51:12", "2:51:12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("03:42:34", "3:42:34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("04:33:56", "4:33:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("05:24:23", "5:24:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("01:15:45", "1:15:45", DigitsFormat.SingleDigitHours)]
        [DataRow("02:56:51", "02:56:51", DigitsFormat.DoubleDigitHours)]

        [DataRow("22:51:12", "22:51:12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("23:42:34", "23:42:34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("14:33:56", "14:33:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("15:24:23", "15:24:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("21:15:45", "21:15:45", DigitsFormat.SingleDigitHours)]
        [DataRow("22:56:51", "22:56:51", DigitsFormat.DoubleDigitHours)]
        
        [DataRow("1:22:51:12", "46:51:12", DigitsFormat.SingleDigitSeconds)]
        [DataRow("1:23:42:34", "47:42:34", DigitsFormat.DoubleDigitSeconds)]
        [DataRow("1:14:33:56", "38:33:56", DigitsFormat.SingleDigitMinutes)]
        [DataRow("1:15:24:23", "39:24:23", DigitsFormat.DoubleDigitMinutes)]
        [DataRow("1:21:15:45", "45:15:45", DigitsFormat.SingleDigitHours)]
        [DataRow("1:22:56:51", "46:56:51", DigitsFormat.DoubleDigitHours)]
        public void TestDigitsFormat(string timespanText, string expected, DigitsFormat format)
        {
            var formatter = new GeneralTimeFormatter();
            formatter.DigitsFormat = format;
            formatter.Accuracy = TimeAccuracy.Seconds;

            TimeSpan? time = null;
            if (timespanText != null)
                time = TimeSpan.Parse(timespanText);

            string formatted = formatter.Format(time);
            Assert.AreEqual(expected, formatted);
        }
    }
}
