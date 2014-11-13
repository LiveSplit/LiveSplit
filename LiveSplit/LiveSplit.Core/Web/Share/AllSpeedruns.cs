using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.Share
{
    public class AllSpeedRuns : ASUPRunUploadPlatform
    {
        protected static AllSpeedRuns _Instance = new AllSpeedRuns();

        public static AllSpeedRuns Instance { get { return _Instance; } }

        protected AllSpeedRuns()
            : base(
            "AllSpeedRuns", "http://allspeedruns.com/", "asup/",
            "AllSpeedRuns is a useful platform for keeping track of your Personal Bests.")
        { }
    }
}
