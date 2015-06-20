using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RecordsClient
    {
        private SpeedrunComClient baseClient;

        public RecordsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }
    }
}
