using System;

using LiveSplit.Model;
using LiveSplit.Web.SRL.RaceViewers;

namespace LiveSplit.Web.SRL;

public interface IRaceViewer
{
    string Name { get; }
    void ShowRace(IRaceInfo race);
}

public static class RaceViewer
{
    public static IRaceViewer FromName(string name)
    {
        return name switch
        {
            "Kadgar" => new Kadgar(),
            "MultiTwitch" => new MultiTwitch(),
            "Speedrun.tv" => new SpeedrunTV(),
            "SpeedRunsLive" => new SRLRaceViewer(),
            _ => throw new ArgumentException("name"),
        };
    }
}
