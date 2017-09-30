using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;

namespace LiveSplit.Options
{
    public interface ISettings : ICloneable
    {
        IDictionary<string, HotkeyProfile> HotkeyProfiles { get; set; }
        KeyOrButton ScrollUp { get; set; }
        KeyOrButton ScrollDown { get; set; }
        IList<RecentSplitsFile> RecentSplits { get; set; }
        IList<string> RecentLayouts { get; set; }
        string LastComparison { get; set; }

        bool WarnOnReset { get; set; }
        bool SimpleSumOfBest { get; set; }
        IRaceViewer RaceViewer { get; set; }
        IList<string> ActiveAutoSplitters { get; set; }
        IDictionary<string, bool> ComparisonGeneratorStates { get; set; }

        bool AgreedToSRLRules { get; set; }

        void AddToRecentSplits(string path, IRun run, TimingMethod lastTimingMethod, string lastHotkeyProfile);
        void AddToRecentLayouts(string path);
        void RegisterHotkeys(CompositeHook hook, string hotkeyProfileName);
        void UnregisterAllHotkeys(CompositeHook hook);
    }
}
