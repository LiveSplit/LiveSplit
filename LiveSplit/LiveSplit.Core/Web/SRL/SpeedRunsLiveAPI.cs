using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace LiveSplit.Web.SRL
{
    public class SpeedRunsLiveAPI : RaceProviderAPI
    {
        protected static readonly SpeedRunsLiveAPI _Instance = new SpeedRunsLiveAPI();

        public static SpeedRunsLiveAPI Instance => _Instance;
        public static readonly Uri BaseUri = new Uri("http://api.speedrunslive.com:81/");

        protected IEnumerable<SRLRaceInfo> racesList;
        protected IEnumerable<dynamic> gameList;
        protected IList<string> gameNames;
        protected IDictionary<string, Image> imageList;

        protected SpeedRunsLiveAPI()
        {
            imageList = new Dictionary<string, Image>();
        }

        protected Uri GetUri(string subUri)
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

        public IEnumerable<string> GetGameNames()
        {
            if (gameNames == null)
            {
                Func<dynamic, string> map = x => x.name;
                gameNames = GetGameList().Select(map).ToList();
            }
            return gameNames;
        }

        public IEnumerable<string> GetCategories(string gameID)
        {
            if (string.IsNullOrEmpty(gameID))
                return new string[0];

            return ((IEnumerable<dynamic>)JSON.FromUri(GetUri("goals/" + gameID + "?season=0")).goals).Select(x => (string)x.name);
        }

        public string GetGameIDFromName (string name)
        {
            Func<dynamic, bool> map = x => x.name == name;
            var gameID = GetGameList().Where(map).FirstOrDefault();
            if (gameID != null)
                return gameID.abbrev;
            return null;
        }

        public IEnumerable<dynamic> GetEntrants(string raceID)
        {
            var race = GetRace(raceID);
            return race.entrants;
        }

        public dynamic GetRace(string raceID)
        {
            var races = GetRaces();
            return races.First(x => x.Id == raceID);
        }

        public override IEnumerable<IRaceInfo> GetRaces()
        {
            if (racesList == null)
                RefreshRacesList();

            return racesList;
        }

        public override Image GetGameImage(string gameId)
        {
            lock (imageList)
            {
                if (!imageList.ContainsKey(gameId))
                {
                    Image image = null;

                    try
                    {
                        var request = WebRequest.Create($"http://c15111072.r72.cf2.rackcdn.com/{gameId}.jpg");

                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        {
                            image = Image.FromStream(stream);
                        }
                    }
                    finally
                    {
                        if (!imageList.ContainsKey(gameId))
                            imageList.Add(gameId, image);
                    }
                }

                return imageList[gameId];
            }
        }

        void SpeedRunsLiveAPI_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RefreshRacesList();
            }
            catch { }
        }

        public override void RefreshRacesListAsync()
        {
            Task.Factory.StartNew(() => RefreshRacesList())
                .ContinueWith((raceItem) => { }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void RefreshRacesList()
        {
            List<SRLRaceInfo> infoList = new List<SRLRaceInfo>();
            int finishedCount, forfeitedCount;
            foreach (var race in JSON.FromUri(GetUri("races")).races)
            {
                infoList.Add(new SRLRaceInfo(race));
            }

            racesList = infoList;
            RacesRefreshedCallback?.Invoke(this);
        }

        public override string ProviderName => "SRL";

        public override string Username
        {
            get
            {
                ShareSettings.Default.Reload();
                return WebCredentials.SpeedRunsLiveIRCCredentials.Username;
            }
        }
    }
}

