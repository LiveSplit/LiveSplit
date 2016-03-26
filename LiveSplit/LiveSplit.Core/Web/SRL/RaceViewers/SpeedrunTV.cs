using System.Diagnostics;

namespace LiveSplit.Web.SRL.RaceViewers
{
    public class SpeedrunTV : IRaceViewer
    {
        public void ShowRace(dynamic race)
        {
            var raceId = race.id;
            var url = string.Format("http://speedrun.tv/race:{0}", raceId);
            Process.Start(url);
        }

        public string Name => "Speedrun.tv";
    }
}
