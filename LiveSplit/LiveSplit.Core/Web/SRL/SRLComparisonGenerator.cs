using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using System;

namespace LiveSplit.Web.SRL
{
    public class SRLComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public string Name { get; protected set; }

        public SRLComparisonGenerator(String name)
        {
            Name = name;
        }

        public void Generate(ISettings settings)
        {
        }
    }
}
