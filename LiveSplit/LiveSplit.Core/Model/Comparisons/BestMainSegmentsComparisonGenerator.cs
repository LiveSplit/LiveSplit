using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons
{
    public class BestMainSegmentsComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Best Main Segments";
        public const string ShortComparisonName = "Mains";

        public string Name => ComparisonName;

        public BestMainSegmentsComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(ISettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
