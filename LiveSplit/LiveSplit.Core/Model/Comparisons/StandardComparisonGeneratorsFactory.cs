using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public class StandardComparisonGeneratorsFactory : IComparisonGeneratorsFactory
    {
        static StandardComparisonGeneratorsFactory()
        {
            CompositeComparisons.AddShortComparisonName(BestSegmentsComparisonGenerator.ComparisonName, "Best");
            CompositeComparisons.AddShortComparisonName(Run.PersonalBestComparisonName, "PB");
            CompositeComparisons.AddShortComparisonName(MedianSegmentsComparisonGenerator.ComparisonName, "Average");
        }
        public IEnumerable<IComparisonGenerator> Create(IRun run)
        {
            yield return new BestSegmentsComparisonGenerator(run);
            yield return new BestSplitTimesComparisonGenerator(run);
            yield return new MedianSegmentsComparisonGenerator(run);
            yield return new NoneComparisonGenerator(run);
        }
    }
}
