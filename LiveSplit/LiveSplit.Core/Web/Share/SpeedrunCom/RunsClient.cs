using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RunsClient
    {
        private SpeedrunComClient baseClient;

        public RunsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public ReadOnlyCollection<Run> GetRuns(int? gameId = null, int? categoryId = null)
        {
            throw new NotImplementedException();
        }
    }
}
