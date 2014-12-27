using LiveSplit.TimeFormatters;
using System;
using System.Globalization;

namespace LiveSplit.Web.Share
{
    public class ASUPTimeFormatter : ITimeFormatter
    {
        public string Format(TimeSpan? time)
        {
            if (!time.HasValue)
                return Format(TimeSpan.Zero);

            return time.Value.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }
    }
}
