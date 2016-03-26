using System.Diagnostics;
using System.Text;

namespace LiveSplit.Web.SRL.RaceViewers
{
    public class Kadgar : IRaceViewer
    {
        public void ShowRace(dynamic race)
        {
            var builder = new StringBuilder();
            builder.Append("http://kadgar.net/live/");
            foreach (var entrant in race.entrants.Properties.Values)
            {
                if (entrant.statetext == "Forfeit" || entrant.time >= 0)
                    continue;

                var stream = entrant.twitch;
                builder.Append(stream);
                builder.Append(",");
            }
            builder.Length -= 1;
            Process.Start(builder.ToString());
        }

        public string Name => "Kadgar";
    }
}
