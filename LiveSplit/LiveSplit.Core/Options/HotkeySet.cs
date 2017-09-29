using LiveSplit.Model.Input;
using System;
using System.Xml;

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

        public static HotkeySet FromXml(XmlElement element, Version version)
        {
            var hotkeySet = new HotkeySet();

            var keyStart = element["SplitKey"];
            if (!string.IsNullOrEmpty(keyStart.InnerText))
                hotkeySet.SplitKey = new KeyOrButton(keyStart.InnerText);
            else
                hotkeySet.SplitKey = null;

            var keyReset = element["ResetKey"];
            if (!string.IsNullOrEmpty(keyReset.InnerText))
                hotkeySet.ResetKey = new KeyOrButton(keyReset.InnerText);
            else
                hotkeySet.ResetKey = null;

            var keySkip = element["SkipKey"];
            if (!string.IsNullOrEmpty(keySkip.InnerText))
                hotkeySet.SkipKey = new KeyOrButton(keySkip.InnerText);
            else
                hotkeySet.SkipKey = null;

            var keyUndo = element["UndoKey"];
            if (!string.IsNullOrEmpty(keyUndo.InnerText))
                hotkeySet.UndoKey = new KeyOrButton(keyUndo.InnerText);
            else
                hotkeySet.UndoKey = null;

            if (version >= new Version(1, 0))
            {
                var keyPause = element["PauseKey"];
                if (!string.IsNullOrEmpty(keyPause.InnerText))
                    hotkeySet.PauseKey = new KeyOrButton(keyPause.InnerText);
                else
                    hotkeySet.PauseKey = null;

                var keyToggle = element["ToggleGlobalHotkeys"];
                if (!string.IsNullOrEmpty(keyToggle.InnerText))
                    hotkeySet.ToggleGlobalHotkeys = new KeyOrButton(keyToggle.InnerText);
                else
                    hotkeySet.ToggleGlobalHotkeys = null;

                if (version >= new Version(1, 3))
                {
                    var keySwitchPrevious = element["SwitchComparisonPrevious"];
                    if (!string.IsNullOrEmpty(keySwitchPrevious.InnerText))
                        hotkeySet.SwitchComparisonPrevious = new KeyOrButton(keySwitchPrevious.InnerText);
                    else
                        hotkeySet.SwitchComparisonPrevious = null;

                    var keySwitchNext = element["SwitchComparisonNext"];
                    if (!string.IsNullOrEmpty(keySwitchNext.InnerText))
                        hotkeySet.SwitchComparisonNext = new KeyOrButton(keySwitchNext.InnerText);
                    else
                        hotkeySet.SwitchComparisonNext = null;
                }
            }

            return hotkeySet;
        }

        public XmlElement ToXml(XmlDocument document, string name = "Hotkey Set")
        {
            var parent = document.CreateElement(name);

            var splitKey = document.CreateElement("SplitKey");
            if (SplitKey != null)
                splitKey.InnerText = SplitKey.ToString();
            parent.AppendChild(splitKey);

            var resetKey = document.CreateElement("ResetKey");
            if (ResetKey != null)
                resetKey.InnerText = ResetKey.ToString();
            parent.AppendChild(resetKey);

            var skipKey = document.CreateElement("SkipKey");
            if (SkipKey != null)
                skipKey.InnerText = SkipKey.ToString();
            parent.AppendChild(skipKey);

            var undoKey = document.CreateElement("UndoKey");
            if (UndoKey != null)
                undoKey.InnerText = UndoKey.ToString();
            parent.AppendChild(undoKey);

            var pauseKey = document.CreateElement("PauseKey");
            if (PauseKey != null)
                pauseKey.InnerText = PauseKey.ToString();
            parent.AppendChild(pauseKey);

            var toggleKey = document.CreateElement("ToggleGlobalHotkeys");
            if (ToggleGlobalHotkeys != null)
                toggleKey.InnerText = ToggleGlobalHotkeys.ToString();
            parent.AppendChild(toggleKey);

            var switchComparisonPrevious = document.CreateElement("SwitchComparisonPrevious");
            if (SwitchComparisonPrevious != null)
                switchComparisonPrevious.InnerText = SwitchComparisonPrevious.ToString();
            parent.AppendChild(switchComparisonPrevious);

            var switchComparisonNext = document.CreateElement("SwitchComparisonNext");
            if (SwitchComparisonNext != null)
                switchComparisonNext.InnerText = SwitchComparisonNext.ToString();
            parent.AppendChild(switchComparisonNext);

            return parent;
        }

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
