using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Options;

namespace LiveSplit.Racetime;

internal class RacetimeComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public string Name { get; protected set; }

    public RacetimeComparisonGenerator(string name)
    {
        Name = name;
    }

    public void Generate(ISettings settings)
    {
    }

    public static string GetRaceComparisonName(string user)
    {
        return "[Race] " + user;
    }

    public static bool IsRaceComparison(string comparison)
    {
        return comparison.StartsWith("[Race] ");
    }
}
