using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using LiveSplit.UI.Components;

namespace LiveSplit.Options
{
    public class Settings : ISettings
    {
        public IDictionary<string, HotkeyProfile> HotkeyProfiles { get; set; }
        public IList<RecentSplitsFile> RecentSplits { get; set; }
        public IList<string> RecentLayouts { get; set; }
        public string LastComparison { get; set; }
        public bool WarnOnReset { get; set; }
        public bool AgreedToSRLRules { get; set; }
        public bool SimpleSumOfBest { get; set; }
        public IRaceViewer RaceViewer { get; set; }
        public IList<RaceProviderSettings> RaceProvider { get; set; }
        public IList<string> ActiveAutoSplitters { get; set; }
        public IDictionary<string, bool> ComparisonGeneratorStates { get; set; }

        // Deprecated properties
        public KeyOrButton SplitKey {
            get { return HotkeyProfiles.First().Value.SplitKey; }
            set { HotkeyProfiles.First().Value.SplitKey = value; }
        }
        public KeyOrButton ResetKey {
            get { return HotkeyProfiles.First().Value.ResetKey; }
            set { HotkeyProfiles.First().Value.ResetKey = value; }
        }
        public KeyOrButton SkipKey {
            get { return HotkeyProfiles.First().Value.SkipKey; }
            set { HotkeyProfiles.First().Value.SkipKey = value; }
        }
        public KeyOrButton UndoKey {
            get { return HotkeyProfiles.First().Value.UndoKey; }
            set { HotkeyProfiles.First().Value.UndoKey = value; }
        }
        public KeyOrButton PauseKey {
            get { return HotkeyProfiles.First().Value.PauseKey; }
            set { HotkeyProfiles.First().Value.PauseKey = value; }
        }
        public KeyOrButton ToggleGlobalHotkeys {
            get { return HotkeyProfiles.First().Value.ToggleGlobalHotkeys; }
            set { HotkeyProfiles.First().Value.ToggleGlobalHotkeys = value; }
        }
        public KeyOrButton SwitchComparisonPrevious {
            get { return HotkeyProfiles.First().Value.SwitchComparisonPrevious; }
            set { HotkeyProfiles.First().Value.SwitchComparisonPrevious = value; }
        }
        public KeyOrButton SwitchComparisonNext {
            get { return HotkeyProfiles.First().Value.SwitchComparisonNext; }
            set { HotkeyProfiles.First().Value.SwitchComparisonNext = value; }
        }
        public float HotkeyDelay {
            get { return HotkeyProfiles.First().Value.HotkeyDelay; }
            set { HotkeyProfiles.First().Value.HotkeyDelay = value; }
        }
        public bool GlobalHotkeysEnabled {
            get { return HotkeyProfiles.First().Value.GlobalHotkeysEnabled; }
            set { HotkeyProfiles.First().Value.GlobalHotkeysEnabled = value; }
        }
        public bool DeactivateHotkeysForOtherPrograms {
            get { return HotkeyProfiles.First().Value.DeactivateHotkeysForOtherPrograms; }
            set { HotkeyProfiles.First().Value.DeactivateHotkeysForOtherPrograms = value; }
        }
        public bool DoubleTapPrevention {
            get { return HotkeyProfiles.First().Value.DoubleTapPrevention; }
            set { HotkeyProfiles.First().Value.DoubleTapPrevention = value; }
        }

        public Settings()
        {
            RecentSplits = new List<RecentSplitsFile>();
            RecentLayouts = new List<string>();
            ActiveAutoSplitters = new List<string>();
            RaceProvider = new List<RaceProviderSettings>();
        }

        public object Clone()
        {
            return new Settings()
            {
                HotkeyProfiles = HotkeyProfiles.ToDictionary(x => x.Key, x => (HotkeyProfile)(x.Value.Clone())),
                WarnOnReset = WarnOnReset,
                RecentSplits = new List<RecentSplitsFile>(RecentSplits),
                RecentLayouts = new List<string>(RecentLayouts),
                LastComparison = LastComparison,
                RaceViewer = RaceViewer,
                RaceProvider = new List<RaceProviderSettings>(RaceProvider),
                AgreedToSRLRules = AgreedToSRLRules,
                SimpleSumOfBest = SimpleSumOfBest,
                ActiveAutoSplitters = new List<string>(ActiveAutoSplitters),
                ComparisonGeneratorStates = new Dictionary<string, bool>(ComparisonGeneratorStates)
            };
        }

        public void RegisterHotkeys(CompositeHook hook, string hotkeyProfileName)
        {
            try
            {
                UnregisterAllHotkeys(hook);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            if (HotkeyProfiles.ContainsKey(hotkeyProfileName))
            {
                var hotkeyProfile = HotkeyProfiles[hotkeyProfileName];
                var deactivateForOtherPrograms = hotkeyProfile.GlobalHotkeysEnabled && hotkeyProfile.DeactivateHotkeysForOtherPrograms;
                if (hotkeyProfile.SplitKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.SplitKey, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.ResetKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.ResetKey, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.SkipKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.SkipKey, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.UndoKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.UndoKey, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.PauseKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.PauseKey, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.ToggleGlobalHotkeys != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.ToggleGlobalHotkeys, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.SwitchComparisonPrevious != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.SwitchComparisonPrevious, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeyProfile.SwitchComparisonNext != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeyProfile.SwitchComparisonNext, deactivateForOtherPrograms);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                hook.AllowGamepads = hotkeyProfile.AllowGamepadsAsHotkeys;
            }
        }

        public void AddToRecentSplits(string path, IRun run, TimingMethod lastTimingMethod, string lastHotkeyProfile)
        {
            var foundRecentSplitsFile = RecentSplits.FirstOrDefault(x => x.Path == path);
            if (foundRecentSplitsFile.Path != null)
                RecentSplits.Remove(foundRecentSplitsFile);

            var recentSplitsFile = new RecentSplitsFile(path, run, lastTimingMethod, lastHotkeyProfile);

            RecentSplits.Add(recentSplitsFile);

            while (RecentSplits.Count > 50)
                RecentSplits.RemoveAt(0);
        }

        public void AddToRecentLayouts(string path)
        {
            if (RecentLayouts.Contains(path))
                RecentLayouts.Remove(path);
            RecentLayouts.Add(path);
            while (RecentLayouts.Count > 10)
                RecentLayouts.RemoveAt(0);
        }

        private void RegisterHotkey(CompositeHook hook, KeyOrButton key, bool deactivateForOtherPrograms)
        {
            hook.RegisterHotKey(key);
            if (deactivateForOtherPrograms && key.IsKey)
            {
                var args = new System.Windows.Forms.KeyEventArgs(key.Key);
                var modifiers = (args.Alt ? ModifierKeys.Alt : ModifierKeys.None)
                    | (args.Shift ? ModifierKeys.Shift : ModifierKeys.None)
                    | (args.Control ? ModifierKeys.Control : ModifierKeys.None);
                HotkeyHook.Instance.RegisterHotKey(modifiers, args.KeyCode);
            }
        }

        public void UnregisterAllHotkeys(CompositeHook hook)
        {
            hook.UnregisterAllHotkeys();
            HotkeyHook.Instance.UnregisterAllHotkeys();
        }
    }
}
