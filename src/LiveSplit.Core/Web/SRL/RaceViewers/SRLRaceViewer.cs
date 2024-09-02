using System.Diagnostics;

using LiveSplit.Model;

namespace LiveSplit.Web.SRL.RaceViewers;

public class SRLRaceViewer : IRaceViewer
{
    public void ShowRace(IRaceInfo race)
    {
        string url = string.Format("http://speedrunslive.com/race/{0}", race.Id);
        Process.Start(url);
    }

    public string Name => "SpeedRunsLive";
}
