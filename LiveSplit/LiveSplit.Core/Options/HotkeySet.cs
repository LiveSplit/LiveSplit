using LiveSplit.Model.Input;
using System;

namespace LiveSplit.Options
{
    public class HotkeySet : ICloneable
    {
        public KeyOrButton SplitKey { get; set; }
        public KeyOrButton ResetKey { get; set; }
        public KeyOrButton SkipKey { get; set; }
        public KeyOrButton UndoKey { get; set; }
        public KeyOrButton PauseKey { get; set; }
        public KeyOrButton ToggleGlobalHotkeys { get; set; }
        public KeyOrButton SwitchComparisonPrevious { get; set; }
        public KeyOrButton SwitchComparisonNext { get; set; }

        public object Clone()
        {
            return new HotkeySet()
            {
                SplitKey = SplitKey,
                ResetKey = ResetKey,
                SkipKey = SkipKey,
                UndoKey = UndoKey,
                PauseKey = PauseKey,
                ToggleGlobalHotkeys = ToggleGlobalHotkeys,
                SwitchComparisonPrevious = SwitchComparisonPrevious,
                SwitchComparisonNext = SwitchComparisonNext
            };
        }
    }
}
