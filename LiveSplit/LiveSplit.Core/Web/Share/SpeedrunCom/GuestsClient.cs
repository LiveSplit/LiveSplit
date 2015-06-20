using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class GuestsClient
    {
        private SpeedrunComClient baseClient;

        public GuestsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }
    }
}
