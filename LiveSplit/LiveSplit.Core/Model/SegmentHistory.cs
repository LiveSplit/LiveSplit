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
            return this.Min(x => x.Key);
        }
    }
}
