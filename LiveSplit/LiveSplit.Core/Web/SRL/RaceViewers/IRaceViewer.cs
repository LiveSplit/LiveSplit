using LiveSplit.Web.SRL.RaceViewers;
using System;

namespace LiveSplit.Web.SRL
{
    public interface IRaceViewer
    {
        String Name { get; }
        void ShowRace(dynamic race);
    }

    public static class RaceViewer
    {
        public static IRaceViewer FromName(String name)
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
