using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.Comparisons
{
    public interface IComparisonGenerator
    {
        IRun Run { get; set; }
        String Name { get; }
        void Generate(ISettings settings);
    }
}
