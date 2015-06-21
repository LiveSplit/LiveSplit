using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RunTimes
    {
        public TimeSpan? Primary { get; private set; }
        public TimeSpan? RealTime { get; private set; }
        public TimeSpan? RealTimeWithoutLoads { get; private set; }
        public TimeSpan? GameTime { get; private set; }

        private RunTimes() { }

        public static RunTimes Parse(SpeedrunComClient client, dynamic timesElement)
        {
            var times = new RunTimes();

            if (timesElement.primary != null)
                times.Primary = TimeSpan.FromSeconds((double)timesElement.primary_t);

            if (timesElement.realtime != null)
                times.RealTime = TimeSpan.FromSeconds((double)timesElement.realtime_t);

            if (timesElement.realtime_noloads != null)
                times.RealTimeWithoutLoads = TimeSpan.FromSeconds((double)timesElement.realtime_noloads_t);

            if (timesElement.ingame != null)
                times.GameTime = TimeSpan.FromSeconds((double)timesElement.ingame_t);

            return times;
        
        }

        public override string ToString()
        {
            if (Primary.HasValue)
                return Primary.Value.ToString();
            else
                return "-";
        }
    }
}
