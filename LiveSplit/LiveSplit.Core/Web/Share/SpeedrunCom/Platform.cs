using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Platform
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int YearOfRelease { get; private set; }

        public static Platform Parse(SpeedrunComClient client, dynamic platformElement)
        {
            throw new NotImplementedException();
        }
    }
}
