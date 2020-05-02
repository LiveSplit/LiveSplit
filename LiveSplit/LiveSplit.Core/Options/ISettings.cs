using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LiveSplit.Options
{
    public interface ISettings : ICloneable
    {
        IDictionary<string, HotkeyProfile> HotkeyProfiles { get; set; }
        IList<RecentSplitsFile> RecentSplits { get; set; }
        IList<string> RecentLayouts { get; set; }
        string LastComparison { get; set; }

        bool WarnOnReset { get; set; }
        bool SimpleSumOfBest { get; set; }
        IRaceViewer RaceViewer { get; set; }
        IList<RaceProviderSettings> RaceProvider { get; set; }
        
        IList<string> ActiveAutoSplitters { get; set; }
        IDictionary<string, bool> ComparisonGeneratorStates { get; set; }

        bool AgreedToSRLRules { get; set; }

        void AddToRecentSplits(string path, IRun run, TimingMethod lastTimingMethod, string lastHotkeyProfile);
        void AddToRecentLayouts(string path);
        void RegisterHotkeys(CompositeHook hook, string hotkeyProfileName);
        void UnregisterAllHotkeys(CompositeHook hook);

        // Deprecated properties
        KeyOrButton SplitKey { get; set; }
        KeyOrButton ResetKey { get; set; }
        KeyOrButton SkipKey { get; set; }
        KeyOrButton UndoKey { get; set; }
        KeyOrButton PauseKey { get; set; }
        KeyOrButton ToggleGlobalHotkeys { get; set; }
        KeyOrButton SwitchComparisonPrevious { get; set; }
        KeyOrButton SwitchComparisonNext { get; set; }
        float HotkeyDelay { get; set; }
        bool GlobalHotkeysEnabled { get; set; }
        bool DeactivateHotkeysForOtherPrograms { get; set; }
        bool DoubleTapPrevention { get; set; }
    }
}
