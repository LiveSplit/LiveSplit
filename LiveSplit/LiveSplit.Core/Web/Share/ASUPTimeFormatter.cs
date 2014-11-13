using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
