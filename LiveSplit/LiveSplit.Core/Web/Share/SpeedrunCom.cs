using LiveSplit.Model;
using LiveSplit.Options;
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
        public struct Record
        {
            public int? Place;
            public string Runner;
            public Time Time;
            public DateTime? Date;
            public Uri Video;
            public bool RunAvailable { get { return Run != null; } }
            public Lazy<IRun> Run;
        }

        private struct GamePair
        {
            public string Name;
            public string Id;
        }

        protected static readonly SpeedrunCom _Instance = new SpeedrunCom();

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

        private IDictionary<string, dynamic> getWorldRecordList(string game)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}", HttpUtility.UrlPathEncode(game)));
            var response = JSON.FromUri(uri);
            response = (response.Properties.Values as IEnumerable<dynamic>).First();
            return response.Properties as IDictionary<string, dynamic>;
        }

        private IDictionary<string, dynamic> getPersonalBestList(string runner, string game)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}&user={1}",
                HttpUtility.UrlPathEncode(game), HttpUtility.UrlPathEncode(runner)));

            var response = JSON.FromUri(uri);
            response = (response.Properties.Values as IEnumerable<dynamic>).First();
            return response.Properties as IDictionary<string, dynamic>;
        }

        private IDictionary<string, IDictionary<string, dynamic>> getPersonalBestList(string runner)
        {
            var uri = GetAPIUri(string.Format("api_records.php?user={0}", HttpUtility.UrlPathEncode(runner)));
            var response = JSON.FromUri(uri);
            var games = response.Properties as IDictionary<string, dynamic>;
            return games.ToDictionary(x => x.Key, x => x.Value.Properties as IDictionary<string, dynamic>);
        }

        private IDictionary<string, dynamic> getLeaderboards(string game)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}&amount=9999", HttpUtility.UrlPathEncode(game)));
            var response = JSON.FromUri(uri);
            response = (response.Properties.Values as IEnumerable<dynamic>).First();
            return response.Properties as IDictionary<string, dynamic>;
        }

        private Record getWorldRecordEntry(dynamic entry)
        {
            Record record = getRecordEntry(entry);
            record.Place = 1;

            return record;
        }

        private Record getRecordEntry(dynamic entry)
        {
            var runner = entry.player;
            Time time = new Time();

            if (entry.time != null)
            {
                time.RealTime = TimeSpan.FromSeconds(double.Parse(entry.time, CultureInfo.InvariantCulture));
            }

            if ((entry.Properties as IDictionary<string, dynamic>).ContainsKey("timewithloads"))
            {
                //If the game supports Time without Loads, "time" actually returns Time without Loads
                time.GameTime = time.RealTime;

                //Real Time is then stored in timewithloads
                if (entry.timewithloads != null)
                    time.RealTime = TimeSpan.FromSeconds(double.Parse(entry.timewithloads, CultureInfo.InvariantCulture));
                else
                    time.RealTime = null;
            }

            if ((entry.Properties as IDictionary<string, dynamic>).ContainsKey("timeigt"))
            {
                //If there's timeigt, use that as the Game Time instead of Time without Loads
                //since that is more representative of Game Time.
                if (entry.timeigt != null)
                    time.GameTime = TimeSpan.FromSeconds(double.Parse(entry.timeigt, CultureInfo.InvariantCulture));
                else
                    time.GameTime = null;
            }

            Lazy<IRun> run = null;

            if (!string.IsNullOrEmpty(entry.splitsio as string))
            {
                run = new Lazy<IRun>(() =>
                {
                    try
                    {
                        return SplitsIO.Instance.DownloadRunByPath(entry.splitsio as string);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                    return null;
                });
            }

            int? place = null;
            int parsed;
            if (int.TryParse(entry.place as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                place = parsed;

            DateTime? date = null;
            Uri video = null;

            if (!String.IsNullOrEmpty(entry.date))
                date = new DateTime(1970, 1, 1)
                    + TimeSpan.FromSeconds(
                        double.Parse(entry.date,
                        CultureInfo.InvariantCulture));

            if (!String.IsNullOrEmpty(entry.video))
                video = new Uri(entry.video);

            return new Record
            {
                Place = place,
                Time = time,
                Date = date,
                Video = video,
                Runner = runner,
                Run = run
            };
        }

        public IDictionary<string, Record> GetWorldRecordList(string game)
        {
            var recordList = new Dictionary<string, Record>();

            foreach (var entry in getWorldRecordList(game))
            {
                recordList.Add(entry.Key, getWorldRecordEntry(entry.Value));
            }

            return recordList;
        }

        public Record GetWorldRecord(string game, string category)
        {
            try
            {
                var worldRecordList = getWorldRecordList(game);
                return getWorldRecordEntry(worldRecordList[category]);
            }
            catch { }

            return new Record();
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

        public Record GetPersonalBest(string runner, string game, string category)
        {
            try
            {
                var personalBestList = getPersonalBestList(runner, game);
                return getRecordEntry(personalBestList[category]);
            }
            catch { }

            return new Record();
        }

        public IDictionary<string, Record> GetPersonalBestList(string runner, string game)
        {
            var recordList = new Dictionary<string, Record>();

            foreach (var entry in getPersonalBestList(runner, game))
            {
                recordList.Add(entry.Key, getRecordEntry(entry.Value));
            }

            return recordList;
        }

        public IDictionary<string, IDictionary<string, Record>> GetPersonalBestList(string runner)
        {
            var recordList = new Dictionary<string, IDictionary<string, Record>>();

            foreach (var game in getPersonalBestList(runner))
            {
                var categoryList = new Dictionary<string, Record>();

                foreach (var category in game.Value)
                {
                    categoryList.Add(category.Key, getRecordEntry(category.Value));
                }

                recordList.Add(game.Key, categoryList);
            }

            return recordList;
        }

        public IEnumerable<Record> GetLeaderboard(string game, string category)
        {
            var leaderboards = getLeaderboards(game);
            var leaderboard = leaderboards.FirstOrDefault(x => x.Key == category);

            var records = new List<Record>();

            foreach (var recordElement in (leaderboard.Value.Properties as IDictionary<string, dynamic>))
            {
                Record record = getRecordEntry(recordElement.Value);
                record.Place = int.Parse(recordElement.Key, CultureInfo.InvariantCulture);

                records.Add(record);
            }

            return records;
        }

        public IDictionary<string, IEnumerable<Record>> GetLeaderboards(string game)
        {
            var dict = new Dictionary<string, IEnumerable<Record>>();

            var leaderboards = getLeaderboards(game);

            foreach (var category in leaderboards)
            {
                var records = new List<Record>();

                foreach (var recordElement in (category.Value.Properties as IDictionary<string, dynamic>))
                {
                    Record record = getRecordEntry(recordElement.Value);
                    record.Place = int.Parse(recordElement.Key, CultureInfo.InvariantCulture);

                    records.Add(record);
                }

                dict.Add(category.Key, records);
            }

            return dict;
        }
    }
}
