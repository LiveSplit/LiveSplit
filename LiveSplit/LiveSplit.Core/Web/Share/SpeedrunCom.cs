using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace LiveSplit.Web.Share
{
    public class SpeedrunCom
    {
        public struct Record
        {
            public int ID;
            public int? Place;
            public IEnumerable<string> Runners;
            public Time Time;
            public DateTime? Date;
            public Uri Video;
            public bool RunAvailable { get { return Run != null; } }
            public Lazy<IRun> Run;
        }

        private struct GamePair
        {
            public string Name;
            public string ID;
        }

        public enum CoverResolution : int
        {
            x32 = 32,
            x64 = 64,
            x128 = 128,
            x256 = 256
        }

        protected static readonly SpeedrunCom _Instance = new SpeedrunCom();

        public static SpeedrunCom Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("http://www.speedrun.com/");
        public static readonly Uri APIUri = new Uri(BaseUri, "");

        private List<GamePair> gameList;

        protected SpeedrunCom() { }

        public Uri GetSiteUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public Uri GetAPIUri(string subUri)
        {
            return new Uri(APIUri, subUri);
        }

        public Uri GetGameUri(string gameId)
        {
            return GetSiteUri(HttpUtility.UrlPathEncode(gameId));
        }

        public Uri GetCategoryUri(string gameId, string categoryId)
        {
            var uri = string.Format("{0}#{1}", HttpUtility.UrlPathEncode(gameId), HttpUtility.UrlPathEncode(categoryId));
            return GetSiteUri(uri);
        }

        public Uri GetUserUri(string userName)
        {
            var uri = string.Format("user/{0}", HttpUtility.UrlPathEncode(userName));
            return GetSiteUri(uri);
        }

        public Uri GetRunUri(int runId)
        {
            var uri = string.Format("run/{0}", runId);
            return GetSiteUri(uri);
        }

        public Uri GetGameCoverUri(string gameId, CoverResolution resolution = CoverResolution.x128)
        {
            var uri = string.Format("themes/{0}/cover-{1}.png", HttpUtility.UrlPathEncode(gameId), (int)resolution);
            return GetSiteUri(uri);
        }

        private dynamic findCategory(IDictionary<string, dynamic> categories, string categoryName)
        {
            var categoryNameLower = (categoryName ?? "").Trim().ToLowerInvariant();
            return categories.Where(x => (x.Key ?? "").Trim().ToLowerInvariant() == categoryNameLower).First();
        }

        private IEnumerable<GamePair> getGameList()
        {
            if (gameList == null)
            {
                gameList = new List<GamePair>();

                var request = WebRequest.Create(GetSiteUri("games"));
                var stream = request.GetResponse().GetResponseStream();
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    var indexAlphabetic = html.IndexOf("<div class='optionon' id='alphabeticlist' style='display:none;'>");
                    var indexByPlatform = html.IndexOf("<div class='optionon' id='byplatformlist' style='display:none;'>");
                    var alphabeticList = html.Substring(indexAlphabetic, indexByPlatform - indexAlphabetic);
                    var gamesHtml = alphabeticList.Split(new[] { "<a href='/" }, StringSplitOptions.None);

                    foreach (var gameHtml in gamesHtml.Skip(1))
                    {
                        var indexGameId = gameHtml.IndexOf("'");
                        var indexBeginGameName = gameHtml.IndexOf("'>") + 2;
                        var indexEndGameName = gameHtml.IndexOf("</a>");
                        var gameId = HttpUtility.HtmlDecode(gameHtml.Substring(0, indexGameId));
                        var gameName = HttpUtility.HtmlDecode(gameHtml.Substring(indexBeginGameName, indexEndGameName - indexBeginGameName));

                        gameList.Add(new GamePair()
                            {
                                ID = gameId,
                                Name = gameName
                            });
                    }
                }
            }

            return gameList;
        }

        private IDictionary<string, dynamic> getWorldRecordList(string fuzzyGameName, out string actualGameName)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}", HttpUtility.UrlPathEncode(fuzzyGameName)));
            var response = JSON.FromUri(uri);
            var pair = (response.Properties as IDictionary<string, dynamic>).First();
            actualGameName = pair.Key;
            return pair.Value.Properties as IDictionary<string, dynamic>;
        }

        private IDictionary<string, dynamic> getPersonalBestList(string runnerName, string fuzzyGameName, out string actualGameName)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}&user={1}",
                HttpUtility.UrlPathEncode(fuzzyGameName), HttpUtility.UrlPathEncode(runnerName)));

            var response = JSON.FromUri(uri);
            var pair = (response.Properties as IDictionary<string, dynamic>).First();
            actualGameName = pair.Key;
            return pair.Value.Properties as IDictionary<string, dynamic>;
        }

        private IDictionary<string, IDictionary<string, dynamic>> getPersonalBestList(string runnerName)
        {
            var uri = GetAPIUri(string.Format("api_records.php?user={0}", HttpUtility.UrlPathEncode(runnerName)));
            var response = JSON.FromUri(uri);
            var games = response.Properties as IDictionary<string, dynamic>;
            return games.ToDictionary(x => x.Key, x => x.Value.Properties as IDictionary<string, dynamic>);
        }

        private IDictionary<string, dynamic> getLeaderboards(string fuzzyGameName, out string actualGameName)
        {
            var uri = GetAPIUri(string.Format("api_records.php?game={0}&amount=9999", HttpUtility.UrlPathEncode(fuzzyGameName)));
            var response = JSON.FromUri(uri);
            var pair = (response.Properties as IDictionary<string, dynamic>).First();
            actualGameName = pair.Key;
            return pair.Value.Properties as IDictionary<string, dynamic>;
        }

        private Record getWorldRecordEntry(dynamic entry)
        {
            Record record = getRecordEntry(entry);
            record.Place = 1;

            return record;
        }

        private Record getRecordEntry(dynamic entry)
        {
            var runners = new List<string>() { entry.player };
            var properties = entry.Properties as IDictionary<string, dynamic>;
            string runnerKey;

            for (var i = 2; properties.ContainsKey(runnerKey = string.Format("player{0}", i)); ++i)
            {
                runners.Add(properties[runnerKey] as string);
            }

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
            string videoString = entry.video;

            if (!String.IsNullOrEmpty(entry.date))
                date = new DateTime(1970, 1, 1)
                    + TimeSpan.FromSeconds(
                        double.Parse(entry.date,
                        CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(videoString))
            {
                if (!videoString.StartsWith("http"))
                    videoString = "http://" + videoString;
                try
                {
                    video = new Uri(videoString);
                }
                catch { }
            }

            var id = 0;
            int.TryParse(entry.id as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out id);

            return new Record
            {
                ID = id,
                Place = place,
                Time = time,
                Date = date,
                Video = video,
                Runners = runners,
                Run = run
            };
        }

        public TimingMethod GetLeaderboardTimingMethod(IEnumerable<Record> leaderboard)
        {
            var lastRecord = leaderboard.First();

            foreach (var record in leaderboard)
            {
                if (lastRecord.Time.RealTime > record.Time.RealTime)
                    return TimingMethod.GameTime;
                lastRecord = record;
            }

            return TimingMethod.RealTime;
        }

        public IDictionary<string, Record> GetWorldRecordList(string fuzzyGameName, out string actualGameName)
        {
            var recordList = new Dictionary<string, Record>();

            foreach (var entry in getWorldRecordList(fuzzyGameName, out actualGameName))
            {
                recordList.Add(entry.Key, getWorldRecordEntry(entry.Value));
            }

            return recordList;
        }

        public IDictionary<string, Record> GetWorldRecordList(string fuzzyGameName)
        {
            string dummy;
            return GetWorldRecordList(fuzzyGameName, out dummy);
        }

        public Record GetWorldRecord(string fuzzyGameName, string categoryName, out string actualGameName)
        {
            try
            {
                var worldRecordList = getWorldRecordList(fuzzyGameName, out actualGameName);
                var category = findCategory(worldRecordList, categoryName);
                return getWorldRecordEntry(category);
            }
            catch { }

            actualGameName = null;
            return new Record();
        }

        public Record GetWorldRecord(string fuzzyGameName, string categoryName)
        {
            string dummy;
            return GetWorldRecord(fuzzyGameName, categoryName, out dummy);
        }

        public IEnumerable<string> GetGameNames()
        {
            return getGameList().Select(x => x.Name);
        }

        public string GetGameID(string gameName)
        {
            return getGameList().FirstOrDefault(x => x.Name == gameName).ID;
        }

        public string GetCategoryID(string categoryName)
        {
            //TODO: Implement a proper way of figuring out the Category ID
            return categoryName;
        }

        public Image GetGameCover(string gameId)
        {
            var coverUri = GetGameCoverUri(gameId);
            var request = WebRequest.Create(coverUri.AbsoluteUri);
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public IEnumerable<string> GetCategories(string fuzzyGameName, out string actualGameName)
        {
            try
            {
                return getWorldRecordList(fuzzyGameName, out actualGameName).Select(x => x.Key).ToList();
            }
            catch { }

            actualGameName = null;
            return new string[0];
        }

        public string GetActualGameName(string fuzzyGameName)
        {
            string actualGameName;
            getWorldRecordList(fuzzyGameName, out actualGameName);
            return actualGameName;
        }

        public IEnumerable<string> GetCategories(string fuzzyGameName)
        {
            string dummy;
            return GetCategories(fuzzyGameName, out dummy);
        }

        public Record GetPersonalBest(string runnerName, string fuzzyGameName, string categoryName, out string actualGameName)
        {
            try
            {
                var personalBestList = getPersonalBestList(runnerName, fuzzyGameName, out actualGameName);
                var category = findCategory(personalBestList, categoryName);
                return getRecordEntry(category);
            }
            catch { }

            actualGameName = null;
            return new Record();
        }

        public Record GetPersonalBest(string runnerName, string fuzzyGameName, string categoryName)
        {
            string dummy;
            return GetPersonalBest(runnerName, fuzzyGameName, categoryName, out dummy);
        }

        public IDictionary<string, Record> GetPersonalBestList(string runnerName, string fuzzyGameName, out string actualGameName)
        {
            var recordList = new Dictionary<string, Record>();

            foreach (var entry in getPersonalBestList(runnerName, fuzzyGameName, out actualGameName))
            {
                recordList.Add(entry.Key, getRecordEntry(entry.Value));
            }

            return recordList;
        }

        public IDictionary<string, Record> GetPersonalBestList(string runnerName, string fuzzyGameName)
        {
            string dummy;
            return GetPersonalBestList(runnerName, fuzzyGameName, out dummy);
        }

        public IDictionary<string, IDictionary<string, Record>> GetPersonalBestList(string runnerName)
        {
            var recordList = new Dictionary<string, IDictionary<string, Record>>();

            foreach (var game in getPersonalBestList(runnerName))
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

        public IEnumerable<Record> GetLeaderboard(string fuzzyGameName, string categoryName, out string actualGameName)
        {
            var leaderboards = getLeaderboards(fuzzyGameName, out actualGameName);
            var leaderboard = findCategory(leaderboards, categoryName);

            var records = new List<Record>();

            foreach (var recordElement in (leaderboard.Value.Properties as IDictionary<string, dynamic>))
            {
                Record record = getRecordEntry(recordElement.Value);
                record.Place = int.Parse(recordElement.Key, CultureInfo.InvariantCulture);

                records.Add(record);
            }

            return records;
        }

        public IEnumerable<Record> GetLeaderboard(string fuzzyGameName, string categoryName)
        {
            string dummy;
            return GetLeaderboard(fuzzyGameName, categoryName, out dummy);
        }

        public IDictionary<string, IEnumerable<Record>> GetLeaderboards(string fuzzyGameName, out string actualGameName)
        {
            var dict = new Dictionary<string, IEnumerable<Record>>();

            var leaderboards = getLeaderboards(fuzzyGameName, out actualGameName);

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

        public IDictionary<string, IEnumerable<Record>> GetLeaderboards(string fuzzyGameName)
        {
            string dummy;
            return GetLeaderboards(fuzzyGameName, out dummy);
        }
    }
}
