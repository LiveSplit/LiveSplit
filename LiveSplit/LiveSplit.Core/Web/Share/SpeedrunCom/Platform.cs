using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Platform
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int YearOfRelease { get; private set; }

        #region Links

        private Lazy<ReadOnlyCollection<Game>> games;
        private Lazy<ReadOnlyCollection<Run>> runs;

        public ReadOnlyCollection<Game> Games { get { return games.Value; } }
        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }

        #endregion

        private Platform() { }

        public static Platform Parse(SpeedrunComClient client, dynamic platformElement)
        {
            var platform = new Platform();

            //Parse Attributes

            platform.ID = platformElement.id as string;
            platform.Name = platformElement.name as string;
            platform.YearOfRelease = (int)platformElement.released;

            //Parse Links

            platform.games = new Lazy<ReadOnlyCollection<Game>>(() => client.Games.GetGames(platformId: platform.ID).ToList().AsReadOnly());
            platform.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(platformId: platform.ID).ToList().AsReadOnly());

            return platform;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
