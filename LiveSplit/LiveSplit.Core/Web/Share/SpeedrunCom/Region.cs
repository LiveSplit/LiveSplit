using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Region
    {
        public string ID { get; private set; }
        public string Name { get; private set; }

        #region Links

        private Lazy<ReadOnlyCollection<Game>> games;
        private Lazy<ReadOnlyCollection<Run>> runs;

        public ReadOnlyCollection<Game> Games { get { return games.Value; } }
        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }

        #endregion

        private Region() { }

        public static Region Parse(SpeedrunComClient client, dynamic regionElement)
        {
            var region = new Region();

            //Parse Attributes

            region.ID = regionElement.id as string;
            region.Name = regionElement.name as string;

            //Parse Links

            region.games = new Lazy<ReadOnlyCollection<Game>>(() => client.Games.GetGames(regionId: region.ID).ToList().AsReadOnly());
            region.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(regionId: region.ID).ToList().AsReadOnly());

            return region;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
