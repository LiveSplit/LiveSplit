﻿using System;
using System.Drawing;

using LiveSplit.Model.Comparisons;

namespace LiveSplit.Model;

public interface ISegment : ICloneable
{
    Image Icon { get; set; }
    string Name { get; set; }
    Time PersonalBestSplitTime { get; set; }
    IComparisons Comparisons { get; set; }
    Time BestSegmentTime { get; set; }
    Time SplitTime { get; set; }
    SegmentHistory SegmentHistory { get; set; }
}
