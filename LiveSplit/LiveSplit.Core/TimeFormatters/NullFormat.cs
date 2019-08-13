using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.TimeFormatters
{
    public enum NullFormat
    {
        Dash, // "-"
        ZeroWithAccuracy, // "0", "0.0" or "0.00" (dependant on TimeAccuracy; not affected by AutomaticPrecision nor TimeFormat)
        ZeroDotZeroZero, // always "0.00" (used by ShortTimeFormatter)
        ZeroValue, // format the null value the same as TimeSpan.Zero
        Dashes // e.g. "-:--" (like ZeroValue but zeros replaced with dashes)
    }
}
