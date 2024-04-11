using LiveSplit.Web.Share;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveSplit.Web
{
    public class CompositeGameList
    {
        protected static CompositeGameList _Instance = new CompositeGameList();

        public static CompositeGameList Instance => _Instance;

        protected IList<string> gameNames;

        protected IDictionary<string, string> gameIDs;

        protected CompositeGameList()
        { }

        public string GetGameID(string gameName)
        {
            if (gameIDs == null)
            {
                GetGames();
            }

            return gameIDs[gameName];
        }

        public IEnumerable<string> GetGameNames()
        {
            if (gameNames == null)
            {
                GetGames();
            }

            return gameNames;
        }

        private void GetGames()
        {
            var gameNames = new List<string>();
            var gameIDs = new Dictionary<string, string>();

            var headerSet = new HashSet<string>();
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
        }
    }
}
