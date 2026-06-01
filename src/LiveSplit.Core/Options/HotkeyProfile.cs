using LiveSplit.Model.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Options;

public class HotkeyProfile : ICloneable
{
    public KeyOrButton SplitKey { get; set; }
    public KeyOrButton ResetKey { get; set; }
    public KeyOrButton SkipKey { get; set; }
    public KeyOrButton UndoKey { get; set; }
    public KeyOrButton PauseKey { get; set; }
    public KeyOrButton ToggleGlobalHotkeys { get; set; }
    public KeyOrButton SwitchComparisonPrevious { get; set; }
    public KeyOrButton SwitchComparisonNext { get; set; }

    public float HotkeyDelay { get; set; }
    public bool GlobalHotkeysEnabled { get; set; }
    public bool DeactivateHotkeysForOtherPrograms { get; set; }
    public bool DoubleTapPrevention { get; set; }
    public bool AllowGamepadsAsHotkeys { get; set; }

    public const string DefaultHotkeyProfileName = "Default";

    public static HotkeyProfile FromXml(XmlElement element, Version version)
    {
        var hotkeyProfile = new HotkeyProfile();

        XmlElement keyStart = element["SplitKey"];
        hotkeyProfile.SplitKey = !string.IsNullOrEmpty(keyStart.InnerText) ? new KeyOrButton(keyStart.InnerText) : null;

        XmlElement keyReset = element["ResetKey"];
        hotkeyProfile.ResetKey = !string.IsNullOrEmpty(keyReset.InnerText) ? new KeyOrButton(keyReset.InnerText) : null;

        XmlElement keySkip = element["SkipKey"];
        hotkeyProfile.SkipKey = !string.IsNullOrEmpty(keySkip.InnerText) ? new KeyOrButton(keySkip.InnerText) : null;

        XmlElement keyUndo = element["UndoKey"];
        hotkeyProfile.UndoKey = !string.IsNullOrEmpty(keyUndo.InnerText) ? new KeyOrButton(keyUndo.InnerText) : null;

        if (version >= new Version(1, 0))
        {
            XmlElement keyPause = element["PauseKey"];
            hotkeyProfile.PauseKey = !string.IsNullOrEmpty(keyPause.InnerText) ? new KeyOrButton(keyPause.InnerText) : null;

            XmlElement keyToggle = element["ToggleGlobalHotkeys"];
            hotkeyProfile.ToggleGlobalHotkeys = !string.IsNullOrEmpty(keyToggle.InnerText) ? new KeyOrButton(keyToggle.InnerText) : null;

            if (version >= new Version(1, 3))
            {
                XmlElement keySwitchPrevious = element["SwitchComparisonPrevious"];
                hotkeyProfile.SwitchComparisonPrevious = !string.IsNullOrEmpty(keySwitchPrevious.InnerText) ? new KeyOrButton(keySwitchPrevious.InnerText) : null;

                XmlElement keySwitchNext = element["SwitchComparisonNext"];
                hotkeyProfile.SwitchComparisonNext = !string.IsNullOrEmpty(keySwitchNext.InnerText) ? new KeyOrButton(keySwitchNext.InnerText) : null;
            }
        }

        hotkeyProfile.GlobalHotkeysEnabled = ParseBool(element["GlobalHotkeysEnabled"], false);
        hotkeyProfile.DeactivateHotkeysForOtherPrograms = ParseBool(element["DeactivateHotkeysForOtherPrograms"], false);
        hotkeyProfile.DoubleTapPrevention = ParseBool(element["DoubleTapPrevention"], true);
        hotkeyProfile.HotkeyDelay = ParseFloat(element["HotkeyDelay"], 0f);

        hotkeyProfile.AllowGamepadsAsHotkeys = ParseBool(element["AllowGamepadsAsHotkeys"], false);
        if (version < new Version(1, 8, 17) && !hotkeyProfile.AnyGamepadKeys())
        {
            hotkeyProfile.AllowGamepadsAsHotkeys = false;
        }

        return hotkeyProfile;
    }

    public XmlElement ToXml(XmlDocument document, string name = "HotkeyProfile")
    {
        XmlElement parent = document.CreateElement(name);

        XmlElement splitKey = document.CreateElement("SplitKey");
        if (SplitKey != null)
        {
            splitKey.InnerText = SplitKey.ToString();
        }

        parent.AppendChild(splitKey);

        XmlElement resetKey = document.CreateElement("ResetKey");
        if (ResetKey != null)
        {
            resetKey.InnerText = ResetKey.ToString();
        }

        parent.AppendChild(resetKey);

        XmlElement skipKey = document.CreateElement("SkipKey");
        if (SkipKey != null)
        {
            skipKey.InnerText = SkipKey.ToString();
        }

        parent.AppendChild(skipKey);

        XmlElement undoKey = document.CreateElement("UndoKey");
        if (UndoKey != null)
        {
            undoKey.InnerText = UndoKey.ToString();
        }

        parent.AppendChild(undoKey);

        XmlElement pauseKey = document.CreateElement("PauseKey");
        if (PauseKey != null)
        {
            pauseKey.InnerText = PauseKey.ToString();
        }

        parent.AppendChild(pauseKey);

        XmlElement toggleKey = document.CreateElement("ToggleGlobalHotkeys");
        if (ToggleGlobalHotkeys != null)
        {
            toggleKey.InnerText = ToggleGlobalHotkeys.ToString();
        }

        parent.AppendChild(toggleKey);

        XmlElement switchComparisonPrevious = document.CreateElement("SwitchComparisonPrevious");
        if (SwitchComparisonPrevious != null)
        {
            switchComparisonPrevious.InnerText = SwitchComparisonPrevious.ToString();
        }

        parent.AppendChild(switchComparisonPrevious);

        XmlElement switchComparisonNext = document.CreateElement("SwitchComparisonNext");
        if (SwitchComparisonNext != null)
        {
            switchComparisonNext.InnerText = SwitchComparisonNext.ToString();
        }

        parent.AppendChild(switchComparisonNext);

        CreateSetting(document, parent, "GlobalHotkeysEnabled", GlobalHotkeysEnabled);
        CreateSetting(document, parent, "DeactivateHotkeysForOtherPrograms", DeactivateHotkeysForOtherPrograms);
        CreateSetting(document, parent, "DoubleTapPrevention", DoubleTapPrevention);
        CreateSetting(document, parent, "HotkeyDelay", HotkeyDelay);
        CreateSetting(document, parent, "AllowGamepadsAsHotkeys", AllowGamepadsAsHotkeys);

        return parent;
    }

    public object Clone()
    {
        return new HotkeyProfile()
        {
            SplitKey = SplitKey,
            ResetKey = ResetKey,
            SkipKey = SkipKey,
            UndoKey = UndoKey,
            PauseKey = PauseKey,
            ToggleGlobalHotkeys = ToggleGlobalHotkeys,
            SwitchComparisonPrevious = SwitchComparisonPrevious,
            SwitchComparisonNext = SwitchComparisonNext,
            HotkeyDelay = HotkeyDelay,
            GlobalHotkeysEnabled = GlobalHotkeysEnabled,
            DeactivateHotkeysForOtherPrograms = DeactivateHotkeysForOtherPrograms,
            DoubleTapPrevention = DoubleTapPrevention,
            AllowGamepadsAsHotkeys = AllowGamepadsAsHotkeys
        };
    }

    private bool AnyGamepadKeys()
    {
        var buttons = new List<KeyOrButton> {
            SplitKey,
            ResetKey,
            SkipKey,
            UndoKey,
            PauseKey,
            ToggleGlobalHotkeys,
            SwitchComparisonPrevious,
            SwitchComparisonNext
        };

        return buttons.Any(button => button?.IsButton ?? false);
    }
}
