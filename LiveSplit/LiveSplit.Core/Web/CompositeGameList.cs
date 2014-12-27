using LiveSplit.Web.Share;
using LiveSplit.Web.SRL;
using System;
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
                var pbTrackerTask = new Task<IEnumerable<string>>(
                    () =>
                    {
                        try
                        {
                            return PBTracker.Instance.GetGameNames();
                        }
                        catch
                        {
                            return new string[0];
                        }
                    });

                var srlTask = new Task<IEnumerable<string>>(
                    () =>
                    { 
                        try
                        {
                            return SpeedRunsLiveAPI.Instance.GetGameNames();
                        }
                        catch
                        {
                            return new string[0];
                        }
                    });

                var congratsioTask = new Task<IEnumerable<string>>(
                    () =>
                    {
                        try
                        {
                            return Congratsio.Instance.GetGameNames();
                        }
                        catch
                        {
                            return new string[0];
                        }
                    });

                pbTrackerTask.Start();
                srlTask.Start();
                congratsioTask.Start();

                Task.WaitAll(pbTrackerTask, srlTask, congratsioTask);

                gameNames = pbTrackerTask.Result
                    .Concat(srlTask.Result)
                    .Concat(congratsioTask.Result)
                    .Distinct().OrderBy(x => x)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }

            return gameNames;
        }
    }
}
