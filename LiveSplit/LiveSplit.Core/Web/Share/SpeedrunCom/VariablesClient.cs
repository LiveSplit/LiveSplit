using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class VariablesClient
    {
        private SpeedrunComClient baseClient;

        public VariablesClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetVariablesUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("variables{0}", subUri));
        }

        public Variable GetVariable(string variableId)
        {
            var uri = GetVariablesUri(string.Format("/{0}",
                HttpUtility.UrlPathEncode(variableId)));

            var result = JSON.FromUri(uri);

            return Variable.Parse(baseClient, result.data);
        }
    }
}
