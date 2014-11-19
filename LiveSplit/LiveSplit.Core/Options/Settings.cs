using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LiveSplit.Options
{
    public class Settings : ISettings
    {
        public KeyOrButton SplitKey { get; set; }
        public KeyOrButton ResetKey { get; set; }
        public KeyOrButton SkipKey { get; set; }
        public KeyOrButton UndoKey { get; set; }
        public KeyOrButton PauseKey { get; set; }
        public KeyOrButton ToggleGlobalHotkeys { get; set; }
        public KeyOrButton ScrollUp { get; set; }
        public KeyOrButton ScrollDown { get; set; }
        public KeyOrButton SwitchComparisonPrevious { get; set; }
        public KeyOrButton SwitchComparisonNext { get; set; }
        public IList<String> RecentSplits { get; set; }
        public IList<String> RecentLayouts { get; set; }
        public String LastComparison { get; set; }
        public TimingMethod LastTimingMethod { get; set; }
        public float HotkeyDelay { get; set; }
        public bool GlobalHotkeysEnabled { get; set; }
        public bool DeactivateHotkeysForOtherPrograms { get; set; }
        public bool WarnOnReset { get; set; }
        public bool DoubleTapPrevention { get; set; }
        public bool AgreedToSRLRules { get; set; }
        public bool SimpleSumOfBest { get; set; }
        public IRaceViewer RaceViewer { get; set; }
        public IList<String> ActiveAutoSplitters { get; set; }

        public Settings()
        {
            RecentSplits = new List<string>();
            RecentLayouts = new List<string>();
            ActiveAutoSplitters = new List<String>();
        }

        public object Clone()
        {
            return new Settings()
            {
                SplitKey = this.SplitKey,
                ResetKey = this.ResetKey,
                SkipKey = this.SkipKey,
                UndoKey = this.UndoKey,
                PauseKey = this.PauseKey,
                ScrollUp = this.ScrollUp,
                ScrollDown = this.ScrollDown,
                SwitchComparisonPrevious = this.SwitchComparisonPrevious,
                SwitchComparisonNext = this.SwitchComparisonNext,
                ToggleGlobalHotkeys = this.ToggleGlobalHotkeys,
                GlobalHotkeysEnabled = this.GlobalHotkeysEnabled,
                DeactivateHotkeysForOtherPrograms = this.DeactivateHotkeysForOtherPrograms,
                WarnOnReset = this.WarnOnReset,
                DoubleTapPrevention = this.DoubleTapPrevention,
                RecentSplits = new List<String>(this.RecentSplits),
                RecentLayouts = new List<String>(this.RecentLayouts),
                LastComparison = this.LastComparison,
                LastTimingMethod = this.LastTimingMethod,
                HotkeyDelay = this.HotkeyDelay,
                RaceViewer = this.RaceViewer,
                AgreedToSRLRules = this.AgreedToSRLRules,
                SimpleSumOfBest = this.SimpleSumOfBest,
                ActiveAutoSplitters = new List<String>(this.ActiveAutoSplitters)
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

        public void RegisterHotkeys(CompositeHook hook)
        {
            try
            {
                UnregisterAllHotkeys(hook);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            RegisterScrolling(hook);
            if (SplitKey != null)
            {
                try
                {
                    RegisterHotkey(hook, SplitKey);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (ResetKey != null)
            {
                try
                {
                    RegisterHotkey(hook, ResetKey);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (SkipKey != null)
            {
                try
                {
                    RegisterHotkey(hook, SkipKey);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (UndoKey != null)
            {
                try
                {
                    RegisterHotkey(hook, UndoKey);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (PauseKey != null)
            {
                try
                {
                    RegisterHotkey(hook, PauseKey);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (ToggleGlobalHotkeys != null)
            {
                try
                {
                    RegisterHotkey(hook, ToggleGlobalHotkeys);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (SwitchComparisonPrevious != null)
            {
                try
                {
                    RegisterHotkey(hook, SwitchComparisonPrevious);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            if (SwitchComparisonNext != null)
            {
                try
                {
                    RegisterHotkey(hook, SwitchComparisonNext);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }


        public void AddToRecentSplits(string path)
        {
            if (RecentSplits.Contains(path))
                RecentSplits.Remove(path);
            RecentSplits.Add(path);
            while (RecentSplits.Count > 10)
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
