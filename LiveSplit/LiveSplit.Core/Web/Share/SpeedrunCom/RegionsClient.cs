using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RegionsClient
    {
        private SpeedrunComClient baseClient;

        public RegionsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetRegionsUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("regions{0}", subUri));
        }

        public IEnumerable<Region> GetRegions(int? elementsPerPage = null)
        {
            var path = "";
            if (elementsPerPage.HasValue)
                path = "?max=" + elementsPerPage.Value;

            var uri = GetRegionsUri(path);

            return SpeedrunComClient.DoPaginatedRequest<Region>(uri,
                x => Region.Parse(baseClient, x) as Region);
        }

        public Region GetRegion(string regionId)
        {
            var uri = GetRegionsUri(string.Format("/{0}", HttpUtility.UrlPathEncode(regionId)));
            var result = JSON.FromUri(uri);

            return Region.Parse(baseClient, result.data);
        }
    }
}
