using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters {
    public class GeneralTimeFormatter : ITimeFormatter {
        static readonly private CultureInfo ic = CultureInfo.InvariantCulture;

        public TimeAccuracy Accuracy { get; set; }

        //TODO: add enum: BigMinutes (for issue #1336)
        //TODO: add enum: SingleMinutes (for RegularTimeFormatter, e.g. 0:12)
        public TimeFormat TimeFormat { get; set; }

        /// <summary>
        /// How to display null times
        /// </summary>
        public NullFormat NullFormat { get; set; }

        /// <summary>
        /// If true, for example show "1d 23:59:10" instead of "47:59:10". For durations of 24 hours or more, 
        /// </summary>
        public bool ShowDays { get; set; }

        /// <summary>
        /// If true, include a "+" for positive times (excluding zero)
        /// </summary>
        public bool ShowPlus { get; set; }

        /// <summary>
        /// If true, don't display decimals if absolute time is 1 minute or more
        /// </summary>
        public bool DropDecimals { get; set; }

        /// <summary>
        /// If true, don't display trailing zero demical places
        /// </summary>
        public bool AutomaticPrecision { get; set; } = false;

        public GeneralTimeFormatter()
        {
            TimeFormat = TimeFormat.Seconds;
            NullFormat = NullFormat.Dash;
        }

        public string Format(TimeSpan? timeNullable)
        {
            bool isNull = (!timeNullable.HasValue);
            if (isNull) {
                if (NullFormat == NullFormat.Dash) {
                    return TimeFormatConstants.DASH;

                } else if (NullFormat == NullFormat.ZeroWithAccuracy) {
                    return ZeroWithAccuracy();

                } else if (NullFormat == NullFormat.ZeroDotZeroZero) {
                    return "0.00";

                } else if (NullFormat == NullFormat.ZeroValue || NullFormat == NullFormat.Dashes) {
                    timeNullable = TimeSpan.Zero;
                }
            }

            TimeSpan time = timeNullable.Value;

            string minusString;
            if (time < TimeSpan.Zero)
            {
                minusString = TimeFormatConstants.MINUS;
                time = -time;
            }
            else
            {
                minusString = (ShowPlus ? "+" : "");
            }

            string decimalFormat = "";
            if (DropDecimals && time.TotalMinutes >= 1) 
                decimalFormat = "";
            else if (Accuracy == TimeAccuracy.Seconds)
                decimalFormat = "";
            else if (Accuracy == TimeAccuracy.Tenths) 
                decimalFormat = @"\.f";
            else if (Accuracy == TimeAccuracy.Hundredths)
                decimalFormat = @"\.ff";

            if (AutomaticPrecision)
                decimalFormat.Replace('f', 'F');

            string formatted;
            if (time.TotalDays >= 1)
            {
                if (ShowDays)
                    formatted = minusString + time.ToString(@"d\d\ h\:mm\:ss" + decimalFormat, ic);
                else
                    formatted = minusString + (int)time.TotalHours + time.ToString(@"\:mm\:ss" + decimalFormat, ic);
            }
            else if (TimeFormat == TimeFormat.TenHours)
            {
                formatted = minusString + time.ToString(@"hh\:mm\:ss" + decimalFormat, ic);
            }
            else if (time.TotalHours >= 1 || TimeFormat == TimeFormat.Hours)
            {
                formatted = minusString + time.ToString(@"h\:mm\:ss" + decimalFormat, ic);
            }
            else if (TimeFormat == TimeFormat.Minutes)
            {
                formatted = minusString + time.ToString(@"mm\:ss" + decimalFormat, ic);
            }
            else if (time.TotalMinutes >= 1 || TimeFormat == TimeFormat.SingleMinutes)
            {
                formatted = minusString + time.ToString(@"m\:ss" + decimalFormat, ic);
            }
            else
            {
                formatted = minusString + time.ToString(@"%s" + decimalFormat, ic);
            }

            if (isNull && NullFormat == NullFormat.Dashes)
                formatted = formatted.Replace('0', '-');

            return formatted;
        }

        private string ZeroWithAccuracy()
        {
            if (Accuracy == TimeAccuracy.Seconds)
                return "0";
            else if (Accuracy == TimeAccuracy.Tenths)
                return "0.0";
            else
                return "0.00";
        }
    }
}