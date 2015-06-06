using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons
{
    public class NoneComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }

        public const string ComparisonName = "None";
        public string Name
        {
            get { return ComparisonName; }
        }

        public NoneComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(ISettings settings)
        {
            foreach (var segment in Run)
            {
                segment.Comparisons[Name] = default(Time);
            }
        }
    }
}
