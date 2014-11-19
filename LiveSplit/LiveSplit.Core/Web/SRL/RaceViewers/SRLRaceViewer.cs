using System;
using System.Diagnostics;

namespace LiveSplit.Web.SRL.RaceViewers
{
    public class SRLRaceViewer : IRaceViewer
    {
        public void ShowRace(dynamic race)
        {
            var raceId = race.id;
            var url = String.Format("http://speedrunslive.com/race/?id={0}", raceId);
            Process.Start(url);
        }

        public string Name
        {
            get { return "SpeedRunsLive"; }
        }
    }
}
