using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share
{
    public class SpeedrunCom
    {
        public struct WorldRecord
        {
            public string Runner;
            public TimeSpan Time;
            public DateTime? Date;
            public Uri Video;
        }

        protected static SpeedrunCom _Instance = new SpeedrunCom();

        public static SpeedrunCom Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("http://www.speedrun.com/");
        public static readonly Uri APIUri = new Uri(BaseUri, "");

        protected SpeedrunCom() { }

        public Uri GetSiteUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public Uri GetAPIUri(string subUri)
        {
            return new Uri(APIUri, subUri);
        }

        private IDictionary<String, dynamic> getWorldRecordList(string game)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}", HttpUtility.UrlPathEncode(game)));
            var response = JSON.FromUri(uri);
            response = (response.Properties.Values as IEnumerable<dynamic>).First();
            return response.Properties as IDictionary<string, dynamic>;
        }

        private WorldRecord getWorldRecordEntry(dynamic entry)
        {
            var runner = entry.player;
            var time = TimeSpan.FromSeconds(double.Parse(entry.time, CultureInfo.InvariantCulture));

            DateTime? date = null;
            Uri video = null;

            if (!String.IsNullOrEmpty(entry.date))
                date = new DateTime(1970, 1, 1)
                    + TimeSpan.FromSeconds(
                        double.Parse(entry.date,
                        CultureInfo.InvariantCulture));

            if (!String.IsNullOrEmpty(entry.video))
                video = new Uri(entry.video);

            return new WorldRecord
            {
                Time = time,
                Date = date,
                Video = video,
                Runner = runner
            };
        }

        public IDictionary<string, WorldRecord> GetWorldRecordList(string game)
        {
            var recordList = new Dictionary<string, WorldRecord>();

            foreach (var entry in getWorldRecordList(game))
            {
                recordList.Add(entry.Key, getWorldRecordEntry(entry.Value));
            }

            return recordList;
        }

        public WorldRecord GetWorldRecord(string game, string category)
        {
            try
            {
                var worldRecordList = getWorldRecordList(game);
                return getWorldRecordEntry(worldRecordList[category]);
            }
            catch { }

            return new WorldRecord();
        }

        public IEnumerable<string> GetCategories(string game)
        {
            IDictionary<string, dynamic> worldRecordList = null;
            try
            {
                worldRecordList = getWorldRecordList(game);
            }
            catch { }

            if (worldRecordList != null)
            {
                foreach (var entry in worldRecordList)
                {
                    yield return entry.Key;
                }
            }
        }
    }
}
