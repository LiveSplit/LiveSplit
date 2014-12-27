using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share
{
    public class ASUP
    {
        public Uri ServerUri { get; set; }

        public ASUP(Uri serverUri)
        {
            ServerUri = serverUri;
        }

        public class IdPair
        {
            public string Id { get; set; }
            public string Value { get; set; }

            public IdPair(string id, string value)
            {
                Id = id;
                Value = value;
            }
        }

        public class WorldRecord
        {
            public string NickName { get; protected set; }
            public TimeSpan Time { get; protected set; }
            public bool IsAccurate { get; protected set; }

            public WorldRecord(string nickName, TimeSpan time, bool isAccurate = true)
            {
                NickName = nickName;
                Time = time;
                IsAccurate = isAccurate;
            }
        }

        public IEnumerable<IdPair> GetGameList()
        {
            var json = JSON.FromUriPost(ServerUri, 
                "type", "gamelist");

            Func<KeyValuePair<string, dynamic>, IdPair> selector = a => new IdPair(a.Key, a.Value.ToString());
            return ((IDictionary<string, dynamic>)(json.data.Properties)).Select(selector).ToList();
        }

        public IEnumerable<IdPair> GetGameCategories(string gameId)
        {
            var json = JSON.FromUriPost(ServerUri, 
                "type", "gamecategories", 
                "game", gameId);

            Func<KeyValuePair<string, dynamic>, IdPair> selector = a => new IdPair(a.Key, a.Value.ToString());
            return ((IDictionary<string, dynamic>)(json.data.Properties)).Select(selector).ToList();
        }

        public WorldRecord GetWorldRecord(string gameId, string categoryId)
        {
            try
            {
                var json = JSON.FromUriPost(ServerUri,
                    "type", "worldrecord",
                    "game", gameId,
                    "category", categoryId);

                return new WorldRecord(json.name, TimeSpan.Parse(json.time), json.isAccurate);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;
        }

        public bool VerifyLogin(string username, string password)
        {
            try
            {
                var json = JSON.FromUriPost(ServerUri,
                    "type", "verifylogin",
                    "username", username,
                    "password", password);
                return json.result == "success";
            }
            catch (Exception e)
            {
                Log.Error(e);

                return false;
            }
        }

        public dynamic SubmitRun(
            IRun run,
            string username, string password,
            string gameId, string categoryId,
            string version, string comment,
            string video,
            params string[] additionalParams)
        {
            var timeFormatter = new ASUPTimeFormatter();

            var splitsBuilder = new StringBuilder();

            splitsBuilder.Append("[");

            foreach (var segment in run)
            {
                splitsBuilder.Append(timeFormatter.Format(segment.PersonalBestSplitTime.RealTime));
                splitsBuilder.Append(", ");
            }

            splitsBuilder.Length -= 2;
            splitsBuilder.Append("]");

            var elements = new string[]
            {
                "type", "submitrun", 
                "username", username,
                "password", password,
                "game", gameId,
                "category", categoryId,
                "version", version,
                "runtime", timeFormatter.Format(run.Last().PersonalBestSplitTime.RealTime),
                "comment", comment,
                "video", video,
                "splits", splitsBuilder.ToString(),
            }.Concat(additionalParams).ToArray();

            return JSON.FromUriPost(ServerUri, elements);
        }
    }
}
