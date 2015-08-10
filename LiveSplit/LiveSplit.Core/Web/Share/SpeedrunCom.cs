using LiveSplit.Model;
using SpeedrunComSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;

namespace LiveSplit.Web.Share
{
    public static class SpeedrunCom
    {
        public static SpeedrunComClient Client { get; private set; }

        public static ISpeedrunComAuthenticator Authenticator { private get; set; }

        static SpeedrunCom()
        {
            ShareSettings.Default.Reload();
            Client = new SpeedrunComClient("LiveSplit/" + Updates.UpdateHelper.Version, ShareSettings.Default.SpeedrunComAccessToken);
        }

        public static bool MakeSureUserIsAuthenticated()
        {
            if (Client.IsAccessTokenValid)
                return true;

            if (Authenticator == null)
                return false;

            var accessToken = Authenticator.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
                return false;

            Client.AccessToken = accessToken;

            return Client.IsAccessTokenValid;
        }

        public static Time ToTime(this RunTimes times)
        {
            var time = new Time(realTime: times.RealTime);

            if (times.GameTime.HasValue)
                time.GameTime = times.GameTime.Value;
            else if (times.RealTimeWithoutLoads.HasValue)
                time.GameTime = times.RealTimeWithoutLoads.Value;

            return time;
        }
        public static IRun GetRun(this Record record)
        {
            var apiUri = record.SplitsUri.AbsoluteUri;
            var path = apiUri.Substring(apiUri.LastIndexOf("/") + 1);
            return SplitsIO.Instance.DownloadRunByPath(path);
        }

        public static Model.TimingMethod ToLiveSplitTimingMethod(this SpeedrunComSharp.TimingMethod timingMethod)
        {
            switch (timingMethod)
            {
                case SpeedrunComSharp.TimingMethod.RealTime:
                    return Model.TimingMethod.RealTime;
                case SpeedrunComSharp.TimingMethod.GameTime:
                case SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads:
                    return Model.TimingMethod.GameTime;
            }

            throw new ArgumentException("timingMethod");
        }

        public static Image GetBoxartImage(this Assets assets)
        {
            var request = WebRequest.Create(assets.CoverMedium.Uri);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public static void PatchRun(this IRun run, SpeedrunComSharp.Run srdcRun)
        {
            run.GameName = srdcRun.Game.Name;
            run.CategoryName = srdcRun.Category.Name;
            run.Metadata.PlatformName = srdcRun.System.Platform.Name;
            run.Metadata.RegionName = srdcRun.System.Region.Name;
            run.Metadata.UsesEmulator = srdcRun.System.IsEmulated;
            run.Metadata.VariableValueNames = srdcRun.VariableValues.ToDictionary(x => x.Name, x => x.Value);
            run.Metadata.RunID = srdcRun.ID;
        }
    }
}
