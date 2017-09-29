using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;

namespace LiveSplit.Options
{
    public class Settings : ISettings
    {
        public IDictionary<string, HotkeySet> HotkeySets { get; set; }
        public KeyOrButton ScrollUp { get; set; }
        public KeyOrButton ScrollDown { get; set; }
        public IList<RecentSplitsFile> RecentSplits { get; set; }
        public IList<string> RecentLayouts { get; set; }
        public string LastComparison { get; set; }
        public float HotkeyDelay { get; set; }
        public bool GlobalHotkeysEnabled { get; set; }
        public bool DeactivateHotkeysForOtherPrograms { get; set; }
        public bool WarnOnReset { get; set; }
        public bool DoubleTapPrevention { get; set; }
        public bool AgreedToSRLRules { get; set; }
        public bool SimpleSumOfBest { get; set; }
        public IRaceViewer RaceViewer { get; set; }
        public IList<string> ActiveAutoSplitters { get; set; }
        public IDictionary<string, bool> ComparisonGeneratorStates { get; set; }

        public Settings()
        {
            RecentSplits = new List<RecentSplitsFile>();
            RecentLayouts = new List<string>();
            ActiveAutoSplitters = new List<string>();
        }

        public object Clone()
        {
            return new Settings()
            {
                HotkeySets = HotkeySets.ToDictionary(x => x.Key, x => (HotkeySet)(x.Value.Clone())),
                ScrollUp = ScrollUp,
                ScrollDown = ScrollDown,
                GlobalHotkeysEnabled = GlobalHotkeysEnabled,
                DeactivateHotkeysForOtherPrograms = DeactivateHotkeysForOtherPrograms,
                WarnOnReset = WarnOnReset,
                DoubleTapPrevention = DoubleTapPrevention,
                RecentSplits = new List<RecentSplitsFile>(RecentSplits),
                RecentLayouts = new List<string>(RecentLayouts),
                LastComparison = LastComparison,
                HotkeyDelay = HotkeyDelay,
                RaceViewer = RaceViewer,
                AgreedToSRLRules = AgreedToSRLRules,
                SimpleSumOfBest = SimpleSumOfBest,
                ActiveAutoSplitters = new List<string>(ActiveAutoSplitters),
                ComparisonGeneratorStates = new Dictionary<string, bool>(ComparisonGeneratorStates)
            };
        }

        private void RegisterScrolling(CompositeHook hook)
        {
            var mouse = hook.GetMouse();
            var name = mouse.Information.InstanceName + " " + mouse.Information.InstanceGuid;
            ScrollUp = new KeyOrButton(new GamepadButton(name, "Scroll_Up"));
            ScrollDown = new KeyOrButton(new GamepadButton(name, "Scroll_Down"));
            hook.RegisterHotKey(ScrollUp);
            hook.RegisterHotKey(ScrollDown);
        }

        public void RegisterHotkeys(CompositeHook hook, string hotkeySetName)
        {
            try
            {
                UnregisterAllHotkeys(hook);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            try
            {
                RegisterScrolling(hook);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            if (HotkeySets.ContainsKey(hotkeySetName))
            {
                var hotkeySet = HotkeySets[hotkeySetName];
                if (hotkeySet.SplitKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.SplitKey);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.ResetKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.ResetKey);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.SkipKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.SkipKey);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.UndoKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.UndoKey);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.PauseKey != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.PauseKey);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.ToggleGlobalHotkeys != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.ToggleGlobalHotkeys);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.SwitchComparisonPrevious != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.SwitchComparisonPrevious);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (hotkeySet.SwitchComparisonNext != null)
                {
                    try
                    {
                        RegisterHotkey(hook, hotkeySet.SwitchComparisonNext);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        public void AddToRecentSplits(string path, IRun run, TimingMethod lastTimingMethod, string lastHotkeySet)
        {
            var foundRecentSplitsFile = RecentSplits.FirstOrDefault(x => x.Path == path);
            if (foundRecentSplitsFile.Path != null)
                RecentSplits.Remove(foundRecentSplitsFile);

            var recentSplitsFile = new RecentSplitsFile(path, run, lastTimingMethod, lastHotkeySet);

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

        private void RegisterHotkey(CompositeHook hook, KeyOrButton key)
        {
            hook.RegisterHotKey(key);
            if (DeactivateHotkeysForOtherPrograms && key.IsKey && GlobalHotkeysEnabled)
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
