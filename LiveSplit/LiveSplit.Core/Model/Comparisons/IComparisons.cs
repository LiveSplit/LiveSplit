using System;
using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public interface IComparisons : IDictionary<String, Time>, ICloneable
    {
    }
}
