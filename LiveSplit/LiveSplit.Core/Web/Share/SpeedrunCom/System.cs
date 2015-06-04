using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class System
    {
        public int PlatformID { get; private set; }
        public bool IsEmulated { get; private set; }
        public int? RegionID { get; private set; }
    }
}
