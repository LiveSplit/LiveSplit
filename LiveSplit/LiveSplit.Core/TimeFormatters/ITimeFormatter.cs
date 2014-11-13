using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.TimeFormatters
{
    public interface ITimeFormatter
    {
        String Format(TimeSpan? time);
    }
}
