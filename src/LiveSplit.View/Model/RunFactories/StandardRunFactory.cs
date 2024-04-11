using LiveSplit.Model.Comparisons;

namespace LiveSplit.Model.RunFactories
{
    public class StandardRunFactory : IRunFactory
    {
        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory)
            {
                GameName = "",
                CategoryName = ""
            };
            run.AddSegment("");
            return run;
        }
    }
}
