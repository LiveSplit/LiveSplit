using LiveSplit.Options;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace LiveSplit.Web
{
    public class CompositeGameList
    {
        protected static CompositeGameList _Instance = new CompositeGameList();

        public static CompositeGameList Instance => _Instance;

        protected IList<string> gameNames;

        protected IDictionary<string, string> gameIDs;

        private const string CacheFilename = "speedruncom_game_names.json";
        private bool loadedFromAPI = false;

        protected CompositeGameList()
        { }

        public string GetGameID(string gameName)
        {
            if (gameIDs == null && !loadedFromAPI)
            {
                GetGames(true);
            }

            return gameIDs[gameName];
        }

        public IEnumerable<string> GetGameNames(bool loadFromCache)
        {
            if (!loadedFromAPI && (!loadFromCache || gameNames == null))
            {
                GetGames(loadFromCache);
            }

            return gameNames;
        }

        private void GetGames(bool loadFromCache)
        {
            if (loadFromCache && File.Exists(CacheFilename))
            {
                LoadFromCache();
                return;
            }

            var gameNames = new List<string>();
            var gameIDs = new Dictionary<string, string>();

            var headerSet = new HashSet<string>();
            try
            {
                foreach (var header in SpeedrunCom.Client.Games.GetGameHeaders())
                {
                    string name = header.Name, id = header.ID;

                    if (!string.IsNullOrEmpty(name) && headerSet.Add(name))
                    {
                        gameNames.Add(name);
                        gameIDs[name] = id;
                    }
                }
                gameNames.Sort();

                this.gameNames = gameNames;
                this.gameIDs = gameIDs;

                SaveToCache();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                loadedFromAPI = true;
            }
        }

        private void SaveToCache()
        {
            var jsonString = new JavaScriptSerializer().Serialize(gameIDs);
            File.WriteAllText(CacheFilename, jsonString);
        }

        private void LoadFromCache()
        {
            var jsonString = File.ReadAllText(CacheFilename);
            gameIDs = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(jsonString);
            gameNames = gameIDs.Keys.ToArray();
        }
    }
}
