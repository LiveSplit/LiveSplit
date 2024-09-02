using System.Diagnostics;

using LiveSplit.Model;

namespace LiveSplit.Web.SRL.RaceViewers;

public class SpeedrunTV : IRaceViewer
{
    public void ShowRace(IRaceInfo race)
    {
        string raceId = race.Id;
        string url = string.Format("http://speedrun.tv/race:{0}", raceId);
        Process.Start(url);
    }

    public string Name => "Speedrun.tv";
}
