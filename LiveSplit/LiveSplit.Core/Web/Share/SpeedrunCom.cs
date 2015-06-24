using LiveSplit.Model;
using SpeedrunComSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share
{
    public static class SpeedrunCom
    {
        public static SpeedrunComClient Client { get; private set; }

        static SpeedrunCom()
        {
            Client = new SpeedrunComClient("LiveSplit/" + LiveSplit.Updates.UpdateHelper.Version);
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

        public static Time GetTime(this Record record)
        {
            var time = new Time(realTime: record.RealTime);

            if (record.GameTime.HasValue)
                time.GameTime = record.GameTime.Value;
            else if (record.RealTimeWithoutLoads.HasValue)
                time.GameTime = record.RealTimeWithoutLoads.Value;

            return time;
        }

        public static LiveSplit.Model.IRun GetRun(this Record record)
        {
            var apiUri = record.SplitsIOUri.AbsoluteUri;
            var path = apiUri.Substring(apiUri.LastIndexOf("/") + 1);
            return SplitsIO.Instance.DownloadRunByPath(path);
        }
    }
}
