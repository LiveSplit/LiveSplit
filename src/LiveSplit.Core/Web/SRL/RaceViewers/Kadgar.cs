using System.Diagnostics;
using System.Text;

using LiveSplit.Model;

namespace LiveSplit.Web.SRL.RaceViewers;

public class Kadgar : IRaceViewer
{
    public void ShowRace(IRaceInfo race)
    {
        var builder = new StringBuilder();
        builder.Append("http://kadgar.net/live/");
        foreach (string stream in race.LiveStreams)
        {
            builder.Append(stream);
            builder.Append(",");
        }

        builder.Length -= 1;
        Process.Start(builder.ToString());
    }

    public string Name => "Kadgar";
}
