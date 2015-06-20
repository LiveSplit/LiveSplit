using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class VariablesClient
    {
        private SpeedrunComClient baseClient;

        public VariablesClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }
    }
}
