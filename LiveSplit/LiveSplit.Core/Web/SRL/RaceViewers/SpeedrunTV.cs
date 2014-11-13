using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.SRL.RaceViewers
{
    public class SpeedrunTV : IRaceViewer
    {
        public void ShowRace(dynamic race)
        {
            var raceId = race.id;
            var url = String.Format("http://speedrun.tv/race:{0}", raceId);
            Process.Start(url);
        }

        public string Name
        {
            get { return "Speedrun.tv"; }
        }
    }
}
