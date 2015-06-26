using LiveSplit.UI;
using System.Globalization;
using System.Xml;

namespace LiveSplit.Options.SettingsSavers
{
    public class XMLSettingsSaver : ISettingsSaver
    {
        public void Save(ISettings settings, System.IO.Stream stream)
        {
            var document = new XmlDocument();

            XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(docNode);

            var parent = document.CreateElement("Settings");
            var version = document.CreateAttribute("version");
            version.Value = "1.6";
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

            parent.AppendChild(SettingsHelper.ToElement(document, "GlobalHotkeysEnabled", settings.GlobalHotkeysEnabled));
            parent.AppendChild(SettingsHelper.ToElement(document, "DeactivateHotkeysForOtherPrograms", settings.DeactivateHotkeysForOtherPrograms));
            parent.AppendChild(SettingsHelper.ToElement(document, "WarnOnReset", settings.WarnOnReset));
            parent.AppendChild(SettingsHelper.ToElement(document, "DoubleTapPrevention", settings.DoubleTapPrevention));
            parent.AppendChild(SettingsHelper.ToElement(document, "HotkeyDelay", settings.HotkeyDelay));

            parent.AppendChild(SettingsHelper.ToElement(document, "RaceViewer", settings.RaceViewer.Name));

            parent.AppendChild(SettingsHelper.ToElement(document, "AgreedToSRLRules", settings.AgreedToSRLRules));

            var recentSplits = document.CreateElement("RecentSplits");
            foreach (var splits in settings.RecentSplits)
            {
                recentSplits.AppendChild(SettingsHelper.ToElement(document, "SplitsPath", splits));
            }
            parent.AppendChild(recentSplits);
            var recentLayouts = document.CreateElement("RecentLayouts");
            foreach (var layout in settings.RecentLayouts)
            {
                recentLayouts.AppendChild(SettingsHelper.ToElement(document, "LayoutPath", layout));
            }
            parent.AppendChild(recentLayouts);

            parent.AppendChild(SettingsHelper.ToElement(document, "LastComparison", settings.LastComparison));
            parent.AppendChild(SettingsHelper.ToElement(document, "LastTimingMethod", settings.LastTimingMethod));
            parent.AppendChild(SettingsHelper.ToElement(document, "SimpleSumOfBest", settings.SimpleSumOfBest));

            var generatorStates = document.CreateElement("ComparisonGeneratorStates");
            foreach (var generator in settings.ComparisonGeneratorStates)
            {
                var generatorElement = document.CreateElement("Generator");
                var name = document.CreateAttribute("name");
                name.Value = generator.Key;
                generatorElement.Attributes.Append(name);
                generatorElement.InnerText = generator.Value.ToString();
                generatorStates.AppendChild(generatorElement);
            }
            parent.AppendChild(generatorStates);

            var autoSplittersActive = document.CreateElement("ActiveAutoSplitters");
            foreach (var splitter in settings.ActiveAutoSplitters)
            {
                autoSplittersActive.AppendChild(SettingsHelper.ToElement(document, "AutoSplitter", splitter));
            }
            parent.AppendChild(autoSplittersActive);

            document.Save(stream);
        }
    }
}
