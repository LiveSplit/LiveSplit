using System;
using System.Globalization;
using System.Text;

namespace LiveSplit.TimeFormatters;

public class DaysTimeFormatter : ITimeFormatter
{
    public string Format(TimeSpan? time)
    {
        if (time.HasValue)
        {
            var builder = new StringBuilder();

            if (time.Value.TotalDays >= 1)
            {
                builder.Append((int)time.Value.TotalDays).Append("d ");
            }

            if (time.Value.TotalHours >= 1)
            {
                builder.Append(time.Value.ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(time.Value.ToString(@"m\:ss", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        return "0";
    }
}
