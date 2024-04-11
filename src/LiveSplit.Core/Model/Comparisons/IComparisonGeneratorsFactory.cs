using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public interface IComparisonGeneratorsFactory
    {
        IEnumerable<IComparisonGenerator> Create(IRun run);
    }
}
