using LiveSplit.Model;
using System.Diagnostics;

namespace LiveSplit.Web.SRL.RaceViewers
{
    public class SRLRaceViewer : IRaceViewer
    {
        public void ShowRace(IRaceInfo race)
        {
            var url = string.Format("http://speedrunslive.com/race/?id={0}", race.Id);
            Process.Start(url);
        }

        public string Name => "SpeedRunsLive";
    }
}
