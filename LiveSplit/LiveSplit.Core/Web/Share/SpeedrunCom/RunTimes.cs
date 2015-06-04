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
    }
}
