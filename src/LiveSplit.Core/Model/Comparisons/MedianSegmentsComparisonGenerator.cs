using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons;

public class MedianSegmentsComparisonGenerator : SegmentPercentileComparisonGenerator
{
    public new const string ComparisonName = "Median Segments"; //you win glacials
    public new const string ShortComparisonName = "Median";

    public override string Name => ComparisonName;

    public MedianSegmentsComparisonGenerator(IRun run)
        : base(run)
    {
        Percentile = 50;
    }

    public override void Generate(ISettings settings)
    {
        Generate(TimingMethod.RealTime);
        Generate(TimingMethod.GameTime);
    }
}
