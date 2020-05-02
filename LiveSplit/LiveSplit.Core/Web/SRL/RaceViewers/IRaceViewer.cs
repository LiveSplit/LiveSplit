using LiveSplit.Model;
using LiveSplit.Web.SRL.RaceViewers;
using System;

namespace LiveSplit.Web.SRL
{
    public interface IRaceViewer
    {
        string Name { get; }
        void ShowRace(IRaceInfo race);
    }

    public static class RaceViewer
    {
        public static IRaceViewer FromName(string name)
        {
            switch (name)
            {
                case "Kadgar": return new Kadgar();
                case "MultiTwitch": return new MultiTwitch();
                case "Speedrun.tv": return new SpeedrunTV();
                case "SpeedRunsLive": return new SRLRaceViewer();
            }

            throw new ArgumentException("name");
        }
    }
}
