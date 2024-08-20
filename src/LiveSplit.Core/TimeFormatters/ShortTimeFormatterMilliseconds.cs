﻿using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters
{
    public class ShortTimeFormatterMilliseconds : ITimeFormatter
    {
        public ShortTimeFormatterMilliseconds() {}

        public string Format(TimeSpan? time, DigitsFormat format)
        {
            var formatRequest = new GeneralTimeFormatter
            {
                Accuracy = TimeAccuracy.Milliseconds,
                NullFormat = NullFormat.ZeroWithAccuracy,
                DigitsFormat = format,
            };

            return formatRequest.Format(time);
        }

        public string Format(TimeSpan? timeSpan)
        {
            return Format(timeSpan, DigitsFormat.SingleDigitSeconds);
        }
    }
}
