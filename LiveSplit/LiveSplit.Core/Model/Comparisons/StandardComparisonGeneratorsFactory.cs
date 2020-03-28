using System.Collections.Generic;
using static LiveSplit.Model.Comparisons.CompositeComparisons;

namespace LiveSplit.Model.Comparisons
{
    public class StandardComparisonGeneratorsFactory : IComparisonGeneratorsFactory
    {
        static StandardComparisonGeneratorsFactory()
        {
            AddShortComparisonName(BestSegmentsComparisonGenerator.ComparisonName, BestSegmentsComparisonGenerator.ShortComparisonName);
            AddShortComparisonName(Run.PersonalBestComparisonName, "PB");
            AddShortComparisonName(AverageSegmentsComparisonGenerator.ComparisonName, AverageSegmentsComparisonGenerator.ShortComparisonName);
            AddShortComparisonName(WorstSegmentsComparisonGenerator.ComparisonName, WorstSegmentsComparisonGenerator.ShortComparisonName);
            AddShortComparisonName(PercentileComparisonGenerator.ComparisonName, PercentileComparisonGenerator.ShortComparisonName);
            AddShortComparisonName(LatestRunComparisonGenerator.ComparisonName, LatestRunComparisonGenerator.ShortComparisonName);
        }
        public IEnumerable<IComparisonGenerator> Create(IRun run)
        {
            yield return new BestSegmentsComparisonGenerator(run);
            yield return new AverageSegmentsComparisonGenerator(run);
        }

        public IEnumerable<IComparisonGenerator> GetAllGenerators(IRun run)
        {
            yield return new BestSegmentsComparisonGenerator(run);
            yield return new BestSplitTimesComparisonGenerator(run);
            yield return new AverageSegmentsComparisonGenerator(run);
            yield return new MedianSegmentsComparisonGenerator(run);
            yield return new WorstSegmentsComparisonGenerator(run);
            yield return new PercentileComparisonGenerator(run);
            yield return new LatestRunComparisonGenerator(run);
            yield return new NoneComparisonGenerator(run);
        }
    }
}
