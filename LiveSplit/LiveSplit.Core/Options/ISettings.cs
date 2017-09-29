using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;

namespace LiveSplit.Options
{
    public interface ISettings : ICloneable
    {
        IDictionary<string, HotkeySet> HotkeySets { get; set; }
        KeyOrButton ScrollUp { get; set; }
        KeyOrButton ScrollDown { get; set; }
        IList<RecentSplitsFile> RecentSplits { get; set; }
        IList<string> RecentLayouts { get; set; }
        string LastComparison { get; set; }
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

        void AddToRecentSplits(string path, IRun run, TimingMethod lastTimingMethod, string lastHotkeySet);
        void AddToRecentLayouts(string path);
        void RegisterHotkeys(CompositeHook hook, string hotkeySetName);
        void UnregisterAllHotkeys(CompositeHook hook);
    }
}
