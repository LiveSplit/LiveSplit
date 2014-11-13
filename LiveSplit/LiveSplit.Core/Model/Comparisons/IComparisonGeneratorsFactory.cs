using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.Comparisons
{
    public interface IComparisonGeneratorsFactory
    {
        IEnumerable<IComparisonGenerator> Create(IRun run);
    }
}
