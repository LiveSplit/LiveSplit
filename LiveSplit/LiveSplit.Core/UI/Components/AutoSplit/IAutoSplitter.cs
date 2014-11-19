using LiveSplit.Model;
using System;

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
