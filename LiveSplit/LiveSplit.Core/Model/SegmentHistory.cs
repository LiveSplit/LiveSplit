using System;
using System.Collections.Generic;
using System.Linq;

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

        public SegmentHistory Clone() => new SegmentHistory(this);

        object ICloneable.Clone() => Clone();

        public int GetMinIndex()
        {
            if (Count > 0)
                return Math.Min(this.Min(x => x.Key), 1);
            return 1;
        }
    }
}
