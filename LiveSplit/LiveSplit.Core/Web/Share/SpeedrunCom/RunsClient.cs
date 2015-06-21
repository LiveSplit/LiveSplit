using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RunsClient
    {
        private SpeedrunComClient baseClient;

        public RunsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetRunsUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("runs{0}", subUri));
        }

        public IEnumerable<Run> GetRuns(
            string userId = null, string guestName = null,
            string examerUserId = null, string gameId = null,
            string levelId = null, string categoryId = null,
            string platformId = null, string regionId = null,
            bool onlyEmulatedRuns = false, RunStatusType? status = null,
            int? elementsPerPage = null,
            RunEmbeds embeds = default(RunEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            if (!string.IsNullOrEmpty(userId))
                parameters.Add(string.Format("user={0}", HttpUtility.UrlPathEncode(userId)));
            if (!string.IsNullOrEmpty(guestName))
                parameters.Add(string.Format("guest={0}", HttpUtility.UrlPathEncode(guestName)));
            if (!string.IsNullOrEmpty(examerUserId))
                parameters.Add(string.Format("examiner={0}", HttpUtility.UrlPathEncode(examerUserId)));
            if (!string.IsNullOrEmpty(gameId))
                parameters.Add(string.Format("game={0}", HttpUtility.UrlPathEncode(gameId)));
            if (!string.IsNullOrEmpty(levelId))
                parameters.Add(string.Format("level={0}", HttpUtility.UrlPathEncode(levelId)));
            if (!string.IsNullOrEmpty(categoryId))
                parameters.Add(string.Format("category={0}", HttpUtility.UrlPathEncode(categoryId)));
            if (!string.IsNullOrEmpty(platformId))
                parameters.Add(string.Format("platform={0}", HttpUtility.UrlPathEncode(platformId)));
            if (!string.IsNullOrEmpty(regionId))
                parameters.Add(string.Format("region={0}", HttpUtility.UrlPathEncode(regionId)));
            if (onlyEmulatedRuns)
                parameters.Add("emulated=yes");
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case RunStatusType.New:
                        parameters.Add("status=new"); break;
                    case RunStatusType.Rejected:
                        parameters.Add("status=rejected"); break;
                    case RunStatusType.Verified:
                        parameters.Add("status=verified"); break;
                }
            }

            var uri = GetRunsUri(parameters.ToParameters());
            return SpeedrunComClient.DoPaginatedRequest<Run>(uri,
                x => Run.Parse(baseClient, x) as Run);
        }

        public Run GetRun(string runId,
            RunEmbeds embeds = default(RunEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            var uri = GetRunsUri(string.Format("/{0}{1}",
                HttpUtility.UrlPathEncode(runId),
                parameters.ToParameters()));

            var result = JSON.FromUri(uri);

            return Run.Parse(baseClient, result.data);
        }
    }
}
