using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.UI.Components.AutoSplit
{
    public interface IAutoSplitter
    {
        TimeSpan? GetGameTime(LiveSplitState state);
        bool IsGameTimePaused(LiveSplitState state);
        bool ShouldSplit(LiveSplitState state);
        bool ShouldReset(LiveSplitState state);
        bool ShouldStart(LiveSplitState state);
    }
}
