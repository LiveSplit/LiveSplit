using System;
using System.Globalization;
using System.Xml;

namespace LiveSplit.Options.SettingsSavers
{
    public class XMLSettingsSaver : ISettingsSaver
    {
        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        private XmlElement ToElement(XmlDocument document, String name, float value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString(CultureInfo.InvariantCulture);
            return element;
        }

        public void Save(ISettings settings, System.IO.Stream stream)
        {
            var document = new XmlDocument();

            XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(docNode);

            var parent = document.CreateElement("Settings");
            var version = document.CreateAttribute("version");
            version.Value = "1.4";
            parent.Attributes.Append(version);
            document.AppendChild(parent);

            var splitKey = document.CreateElement("SplitKey");
            if (settings.SplitKey != null)
                splitKey.InnerText = settings.SplitKey.ToString();
            parent.AppendChild(splitKey);

            var resetKey = document.CreateElement("ResetKey");
            if (settings.ResetKey != null)
                resetKey.InnerText = settings.ResetKey.ToString();
            parent.AppendChild(resetKey);

            var skipKey = document.CreateElement("SkipKey");
            if (settings.SkipKey != null)
                skipKey.InnerText = settings.SkipKey.ToString();
            parent.AppendChild(skipKey);

            var undoKey = document.CreateElement("UndoKey");
            if (settings.UndoKey != null)
                undoKey.InnerText = settings.UndoKey.ToString();
            parent.AppendChild(undoKey);

            var pauseKey = document.CreateElement("PauseKey");
            if (settings.PauseKey != null)
                pauseKey.InnerText = settings.PauseKey.ToString();
            parent.AppendChild(pauseKey);

            var toggleKey = document.CreateElement("ToggleGlobalHotkeys");
            if (settings.ToggleGlobalHotkeys != null)
                toggleKey.InnerText = settings.ToggleGlobalHotkeys.ToString();
            parent.AppendChild(toggleKey);

            var switchComparisonPrevious = document.CreateElement("SwitchComparisonPrevious");
            if (settings.SwitchComparisonPrevious != null)
                switchComparisonPrevious.InnerText = settings.SwitchComparisonPrevious.ToString();
            parent.AppendChild(switchComparisonPrevious);

            var switchComparisonNext = document.CreateElement("SwitchComparisonNext");
            if (settings.SwitchComparisonNext != null)
                switchComparisonNext.InnerText = settings.SwitchComparisonNext.ToString();
            parent.AppendChild(switchComparisonNext);

            parent.AppendChild(ToElement(document, "GlobalHotkeysEnabled", settings.GlobalHotkeysEnabled));
            parent.AppendChild(ToElement(document, "DeactivateHotkeysForOtherPrograms", settings.DeactivateHotkeysForOtherPrograms));
            parent.AppendChild(ToElement(document, "WarnOnReset", settings.WarnOnReset));
            parent.AppendChild(ToElement(document, "DoubleTapPrevention", settings.DoubleTapPrevention));
            //parent.AppendChild(ToElement(document, "RefreshRate", settings.RefreshRate));
            parent.AppendChild(ToElement(document, "HotkeyDelay", settings.HotkeyDelay));

            parent.AppendChild(ToElement(document, "RaceViewer", settings.RaceViewer.Name));

            parent.AppendChild(ToElement(document, "AgreedToSRLRules", settings.AgreedToSRLRules));

            var recentSplits = document.CreateElement("RecentSplits");
            foreach (var splits in settings.RecentSplits)
            {
                recentSplits.AppendChild(ToElement(document, "SplitsPath", splits));
            }
            parent.AppendChild(recentSplits);
            var recentLayouts = document.CreateElement("RecentLayouts");
            foreach (var layout in settings.RecentLayouts)
            {
                recentLayouts.AppendChild(ToElement(document, "LayoutPath", layout));
            }
            parent.AppendChild(recentLayouts);

            parent.AppendChild(ToElement(document, "LastComparison", settings.LastComparison));
            parent.AppendChild(ToElement(document, "LastTimingMethod", settings.LastTimingMethod));
            parent.AppendChild(ToElement(document, "SimpleSumOfBest", settings.SimpleSumOfBest));

            var autoSplittersActive = document.CreateElement("ActiveAutoSplitters");
            foreach (var splitter in settings.ActiveAutoSplitters)
            {
                autoSplittersActive.AppendChild(ToElement(document, "AutoSplitter", splitter));
            }
            parent.AppendChild(autoSplittersActive);

            document.Save(stream);
        }
    }
}
