using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters;

/// <summary>
/// Formats <see cref="TimeSpan"/>s according to C#'s built-in "c" format, using a hyphen for null TimeSpans.
/// </summary>
public class PreciseTimeFormatter : ITimeFormatter
{
    public string Format(TimeSpan? time)
    {
        return time?.ToString("c", CultureInfo.InvariantCulture) ?? TimeFormatConstants.DASH;
    }
}
