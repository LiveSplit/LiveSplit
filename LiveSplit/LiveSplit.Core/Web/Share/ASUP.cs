using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
            public String Id { get; set; }
            public String Value { get; set; }

            public IdPair(String id, String value)
            {
                Id = id;
                Value = value;
            }
        }

        public class WorldRecord
        {
            public String NickName { get; protected set; }
            public TimeSpan Time { get; protected set; }
            public bool IsAccurate { get; protected set; }

            public WorldRecord(String nickName, TimeSpan time, bool isAccurate = true)
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

            Func<KeyValuePair<String, dynamic>, IdPair> selector = a => new IdPair(a.Key, a.Value.ToString());
            return ((IDictionary<String, dynamic>)(json.data.Properties)).Select(selector).ToList();
        }

        public IEnumerable<IdPair> GetGameCategories(String gameId)
        {
            var json = JSON.FromUriPost(ServerUri, 
                "type", "gamecategories", 
                "game", gameId);

            Func<KeyValuePair<String, dynamic>, IdPair> selector = a => new IdPair(a.Key, a.Value.ToString());
            return ((IDictionary<String, dynamic>)(json.data.Properties)).Select(selector).ToList();
        }

        public WorldRecord GetWorldRecord(String gameId, String categoryId)
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

        public bool VerifyLogin(String username, String password)
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
            String username, String password, 
            String gameId, String categoryId, 
            String version, String comment,
            String video,
            params String[] additionalParams)
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

            var elements = new String[]
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
