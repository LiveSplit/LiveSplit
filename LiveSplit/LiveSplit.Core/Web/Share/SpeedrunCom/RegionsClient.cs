using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RegionsClient
    {
        private SpeedrunComClient baseClient;

        public RegionsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public Region GetRegion(int regionId)
        {
            throw new NotImplementedException();
        }
    }
}
