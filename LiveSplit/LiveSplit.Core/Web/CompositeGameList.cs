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

        public string GetGameID(string gameName) => gameIDs[gameName];
        
        public IEnumerable<string> GetGameNames()
        {
            if (gameNames == null)
            {
                var speedrunComTask = new Task<IEnumerable<string>>(
                    () =>
                    {
                        try
                        {
                            return SpeedrunCom.Client.Games.GetGameHeaders().Select(x => x.Name);
                        }
                        catch
                        {
                            return new string[0];
                        }
                    });

                speedrunComTask.Start();

                Task.WaitAll(speedrunComTask);

                gameNames = speedrunComTask.Result
                    .Distinct().OrderBy(x => x)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }

            return gameNames;
        }
        
        public IList<string> GetGameNamesAndCacheIDs()
        {
            if (gameIDs == null)
            {
                var speedrunComTask = new Task<IDictionary<string, string>>(
                    () =>
                    {
                        try
                        {
                            return SpeedrunCom.Client.Games.GetGameHeaders()
                                .GroupBy(x => x.Name)
                                .Select(x => x.First())
                                .OrderBy(x => x.Name)
                                .Where(x => !string.IsNullOrEmpty(x.Name))
                                .ToDictionary(x => x.Name, x => x.ID);
                        }
                        catch
                        {
                            return new Dictionary<string, string>();
                        }
                    });

                speedrunComTask.Start();

                Task.WaitAll(speedrunComTask);

                gameNames = speedrunComTask.Result.Keys.ToList();
                gameIDs = speedrunComTask.Result;
            }

            return gameNames;
        }
    }
}
