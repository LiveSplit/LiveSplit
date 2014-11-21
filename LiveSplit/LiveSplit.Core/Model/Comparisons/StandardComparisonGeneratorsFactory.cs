using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public class StandardComparisonGeneratorsFactory : IComparisonGeneratorsFactory
    {
        static StandardComparisonGeneratorsFactory()
        {
            CompositeComparisons.AddShortComparisonName(BestSegmentsComparisonGenerator.ComparisonName, BestSegmentsComparisonGenerator.ShortComparisonName);
            CompositeComparisons.AddShortComparisonName(Run.PersonalBestComparisonName, "PB");
            CompositeComparisons.AddShortComparisonName(AverageSegmentsComparisonGenerator.ComparisonName, AverageSegmentsComparisonGenerator.ShortComparisonName);
        }
        public IEnumerable<IComparisonGenerator> Create(IRun run)
        {
            yield return new BestSegmentsComparisonGenerator(run);
            yield return new BestSplitTimesComparisonGenerator(run);
            yield return new AverageSegmentsComparisonGenerator(run);
            yield return new NoneComparisonGenerator(run);
        }
    }
}
