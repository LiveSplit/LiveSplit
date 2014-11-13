using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model.RunFactories
{
    public interface IRunFactory
    {
        IRun Create(IComparisonGeneratorsFactory factory);
    }
}
