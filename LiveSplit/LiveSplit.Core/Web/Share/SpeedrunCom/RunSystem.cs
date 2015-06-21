using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RunSystem
    {
        public string PlatformID { get; private set; }
        public bool IsEmulated { get; private set; }
        public string RegionID { get; private set; }

        private RunSystem() { }

        public static RunSystem Parse(SpeedrunComClient client, dynamic systemElement)
        {
            var system = new RunSystem();

            system.PlatformID = systemElement.platform as string;
            system.IsEmulated = (bool)systemElement.emulated;
            system.RegionID = systemElement.region as string;

            return system;
        }
    }
}
