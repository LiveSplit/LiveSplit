using LiveSplit.Options;
using System;

namespace LiveSplit.Model.Comparisons
{
    public interface IComparisonGenerator
    {
        IRun Run { get; set; }
        string Name { get; }
        void Generate(ISettings settings);
    }
}
