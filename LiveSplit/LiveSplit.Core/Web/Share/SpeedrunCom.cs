using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

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

        private struct GamePair
        {
            public string Name;
            public string Id;
        }

        protected static SpeedrunCom _Instance = new SpeedrunCom();

        public static SpeedrunCom Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("http://www.speedrun.com/");
        public static readonly Uri APIUri = new Uri(BaseUri, "");

        private IEnumerable<GamePair> gameList;

        protected SpeedrunCom() { }

        public Uri GetSiteUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public Uri GetAPIUri(string subUri)
        {
            return new Uri(APIUri, subUri);
        }

        private IEnumerable<HtmlElement> FindChildren(HtmlElement parent, String tagName, String id = null, String className = null)
        {
            return parent.Children
                .OfType<HtmlElement>()
                .Where(x => x.TagName == tagName 
                    && ((id == null) ? true : x.Id == id)
                    && ((className == null) ? true : x.GetAttribute("class") == className));
        }

        private IEnumerable<GamePair> getGameList()
        {
            if (gameList == null)
            {
                var request = WebRequest.Create(GetSiteUri("games"));
                var stream = request.GetResponse().GetResponseStream();
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    var thread = new Thread(() =>
                    {
                        try
                        {
                            var wbc = new WebBrowser();
                            wbc.DocumentText = "";
                            var doc = new HtmlDocument();
                            doc.Write((string)html);

                            var element = FindChildren(doc.Body, "div", id: "foregrounddiv").First();
                            element = FindChildren(element, "div", className: "clearfix").First();
                            element = FindChildren(element, "table").First();
                            element = FindChildren(element, "tbody").First();
                            element = FindChildren(element, "tr").First();
                            element = FindChildren(element, "td").First();
                            element = FindChildren(element, "div", className: "box padding lists").First();
                            element = FindChildren(element, "div", id: "alphabeticlist").First();
                            var allgames = FindChildren(element, "a");
                            gameList = allgames.Select(x =>
                                new GamePair
                                {
                                    Id = x.GetAttribute("href").Substring(1),
                                    Name = HttpUtility.HtmlDecode(x.InnerHtml)
                                }).ToList();
                        }
                        catch (Exception ex) { }
                    }) { ApartmentState = ApartmentState.STA, IsBackground = true };
                    thread.Start();
                    thread.Join();
                }
            }

            return gameList;
        }

        public IEnumerable<string> GetGameNames()
        {
            return getGameList().Select(x => x.Name);
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
