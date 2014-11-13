using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LiveSplit.Web.SRL
{
    public class SpeedRunsLiveAPI
    {
        protected static SpeedRunsLiveAPI _Instance = new SpeedRunsLiveAPI();

        public static SpeedRunsLiveAPI Instance { get { return _Instance; } }
        public static readonly Uri BaseUri = new Uri("http://api.speedrunslive.com:81/");

        protected IEnumerable<dynamic> racesList;
        protected IEnumerable<dynamic> gameList;
        protected IList<String> gameNames;
        protected IDictionary<String, Image> imageList;

        public event EventHandler RacesRefreshed;

        protected SpeedRunsLiveAPI()
        {
            imageList = new Dictionary<String, Image>();
            //new System.Timers.Timer(20 * 1000) { Enabled = true }.Elapsed += SpeedRunsLiveAPI_Elapsed;
        }

        protected Uri GetUri(String subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public IEnumerable<dynamic> GetGameList()
        {
            if (gameList == null)
            {
                gameList = (IEnumerable<dynamic>)JSON.FromUri(GetUri("games")).games;
            }

            return gameList;
        }

        public IEnumerable<String> GetGameNames()
        {
            if (gameNames == null)
            {
                Func<dynamic, string> map = x => x.name;
                gameNames = GetGameList().Select(map).ToList();
            }
            return gameNames;
        }

        public IEnumerable<String> GetCategories(String gameID)
        {
            if (String.IsNullOrEmpty(gameID))
                return new String[0];

            return ((IEnumerable<dynamic>)JSON.FromUri(GetUri("goals/" + gameID + "?season=0")).goals).Select(x => (String)x.name);
        }

        public String GetGameIDFromName (String name)
        {
            Func<dynamic, bool> map = x => x.name == name;
            var gameID = GetGameList().Where(map).FirstOrDefault();
            if (gameID != null)
                return gameID.abbrev;
            return null;
        }

        public IEnumerable<dynamic> GetEntrants(String raceID)
        {
            var race = GetRace(raceID);
            return race.entrants;
        }

        public dynamic GetRace(String raceID)
        {
            var races = GetRaces();
            return races.First(x => x.id == raceID);
        }

        public IEnumerable<dynamic> GetRaces()
        {
            if (racesList == null)
                RefreshRacesList();

            return racesList;
        }

        public Image GetGameImage(String gameId)
        {
            if (!imageList.ContainsKey(gameId))
            {
                try
                {
                    var request = HttpWebRequest.Create(String.Format("http://c15111072.r72.cf2.rackcdn.com/{0}.jpg", gameId));

                    using (var stream = request.GetResponse().GetResponseStream())
                    {
                        var image = Image.FromStream(stream);
                        lock (imageList)
                        {
                            if (!imageList.ContainsKey(gameId))
                            {
                                imageList.Add(gameId, image);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    try
                    {
                        imageList.Add(gameId, null);
                    }
                    catch (Exception exc)
                    {
                        Log.Error(exc);
                    }
                }
            }

            return imageList[gameId];
        }

        void SpeedRunsLiveAPI_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshRacesList();
        }

        public void RefreshRacesListAsync()
        {
            new Thread(() => RefreshRacesList()).Start();
        }

        public void RefreshRacesList()
        {
            try
            {
                racesList = (IEnumerable<dynamic>)JSON.FromUri(GetUri("races")).races;
                if (RacesRefreshed != null)
                    RacesRefreshed(this, null);
            } 
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
