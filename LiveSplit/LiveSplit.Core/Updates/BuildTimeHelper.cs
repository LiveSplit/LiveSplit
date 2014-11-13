using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Updates
{
    public class BuildTimeHelper
    {
        public static DateTime GetBuildTime()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * version.Build +
                TimeSpan.TicksPerSecond * 2 * version.Revision));

            return buildDateTime;
        }
    }
}
