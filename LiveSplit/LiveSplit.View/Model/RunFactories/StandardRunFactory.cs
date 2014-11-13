using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model.RunFactories
{
    public class StandardRunFactory : IRunFactory
    {
        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory)
            {
                new Segment("")
            };
            return run;
        }
    }
}
