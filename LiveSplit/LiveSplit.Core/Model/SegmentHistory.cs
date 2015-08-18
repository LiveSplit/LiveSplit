using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace LiveSplit.Model
{
    public class SegmentHistory : Dictionary<int, Time>, ICloneable
    {        
        public SegmentHistory(SegmentHistory history) : base(history)
        {
        }

        public SegmentHistory()
        {
        }

        public SegmentHistory Clone()
        {
            return new SegmentHistory(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public int GetMinIndex()
        {
            if (this.Count > 0)
                return Math.Min(this.Min(x => x.Key), 1);
            return 1;
        }
    }
}
