using System;

using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons;

public class BestSegmentsComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public const string ComparisonName = "Best Segments";
    public const string ShortComparisonName = "Best";
    public string Name => ComparisonName;

    public BestSegmentsComparisonGenerator(IRun run)
    {
        Run = run;
    }

    public void Generate(ISettings settings)
    {
        var realTimePredictions = new TimeSpan?[Run.Count + 1];
        var gameTimePredictions = new TimeSpan?[Run.Count + 1];
        SumOfBest.CalculateSumOfBest(Run, 0, Run.Count - 1, realTimePredictions, settings.SimpleSumOfBest, false, TimingMethod.RealTime);
        SumOfBest.CalculateSumOfBest(Run, 0, Run.Count - 1, gameTimePredictions, settings.SimpleSumOfBest, false, TimingMethod.GameTime);
        int index = 1;
        foreach (ISegment segment in Run)
        {
            segment.Comparisons[Name] = new Time(
                realTimePredictions[index],
                gameTimePredictions[index]);
            index++;
        }
    }
}
