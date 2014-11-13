using LiveSplit.Web.SRL.RaceViewers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
