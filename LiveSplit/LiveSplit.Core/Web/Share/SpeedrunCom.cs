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

        public static LiveSplit.Model.TimingMethod ToLiveSplitTimingMethod(this SpeedrunComSharp.TimingMethod timingMethod)
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
            using (var stream = WebRequest.Create(assets.CoverMedium).GetResponse().GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }
    }
}
