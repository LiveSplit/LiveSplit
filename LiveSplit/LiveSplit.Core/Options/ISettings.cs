using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;

namespace LiveSplit.Options
{
    public interface ISettings : ICloneable
    {
        KeyOrButton SplitKey { get; set; }
        KeyOrButton ResetKey { get; set; }
        KeyOrButton SkipKey { get; set; }
        KeyOrButton UndoKey { get; set; }
        KeyOrButton PauseKey { get; set; }
        KeyOrButton ToggleGlobalHotkeys { get; set; }
        KeyOrButton ScrollUp { get; set; }
        KeyOrButton ScrollDown { get; set; }
        KeyOrButton SwitchComparisonPrevious { get; set; }
        KeyOrButton SwitchComparisonNext { get; set; }
        IList<string> RecentSplits { get; set; }
        IList<string> RecentLayouts { get; set; }
        string LastComparison { get; set; }
        TimingMethod LastTimingMethod { get; set; }
        float HotkeyDelay { get; set; }
        bool GlobalHotkeysEnabled { get; set; }
        bool DeactivateHotkeysForOtherPrograms { get; set; }
        bool WarnOnReset { get; set; }
        bool DoubleTapPrevention { get; set; }
        bool SimpleSumOfBest { get; set; }
        IRaceViewer RaceViewer { get; set; }
        IList<string> ActiveAutoSplitters { get; set; }
        IDictionary<string, bool> ComparisonGeneratorStates { get; set; }

        bool AgreedToSRLRules { get; set; }

        void AddToRecentSplits(string path);
        void AddToRecentLayouts(string path);
        void RegisterHotkeys(CompositeHook hook);
        void UnregisterAllHotkeys(CompositeHook hook);
    }
}
