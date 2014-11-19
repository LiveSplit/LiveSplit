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

        protected IList<String> gameNames;

        protected CompositeGameList()
        { }

        public IEnumerable<String> GetGameNames()
        {
            if (gameNames == null)
            {
                var pbTrackerTask = new Task<IEnumerable<String>>(
                    () =>
                    {
                        try
                        {
                            return PBTracker.Instance.GetGameNames();
                        }
                        catch
                        {
                            return new String[0];
                        }
                    });

                var srlTask = new Task<IEnumerable<String>>(
                    () =>
                    { 
                        try
                        {
                            return SpeedRunsLiveAPI.Instance.GetGameNames();
                        }
                        catch
                        {
                            return new String[0];
                        }
                    });

                var congratsioTask = new Task<IEnumerable<String>>(
                    () =>
                    {
                        try
                        {
                            return Congratsio.Instance.GetGameNames();
                        }
                        catch
                        {
                            return new String[0];
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
                    .Where(x => !String.IsNullOrEmpty(x))
                    .ToList();
            }

            return gameNames;
        }
    }
}
