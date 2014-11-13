using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    public interface ISegment : ICloneable
    {
        Image Icon { get; set; }
        String Name { get; set; }
        Time PersonalBestSplitTime { get; set; }
        IComparisons Comparisons { get; set; }
        Time BestSegmentTime { get; set; }
        Time SplitTime { get; set; }
        IList<IIndexedTime> SegmentHistory { get; set; }
    }
}
