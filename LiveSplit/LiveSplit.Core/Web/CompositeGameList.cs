using LiveSplit.Web.Share;
using LiveSplit.Web.SRL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveSplit.Web
{
    public class CompositeGameList
    {
        protected static CompositeGameList _Instance = new CompositeGameList();

        public static CompositeGameList Instance { get { return _Instance; } }

        protected IList<string> gameNames;

        protected CompositeGameList()
        { }

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
    }
}
