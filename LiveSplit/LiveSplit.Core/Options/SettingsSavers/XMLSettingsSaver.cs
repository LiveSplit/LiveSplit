using LiveSplit.Model;
using LiveSplit.UI;
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

            SettingsHelper.CreateSetting(document, parent, "GlobalHotkeysEnabled", settings.GlobalHotkeysEnabled);
            SettingsHelper.CreateSetting(document, parent, "DeactivateHotkeysForOtherPrograms", settings.DeactivateHotkeysForOtherPrograms);
            SettingsHelper.CreateSetting(document, parent, "WarnOnReset", settings.WarnOnReset);
            SettingsHelper.CreateSetting(document, parent, "DoubleTapPrevention", settings.DoubleTapPrevention);
            SettingsHelper.CreateSetting(document, parent, "HotkeyDelay", settings.HotkeyDelay);

            SettingsHelper.CreateSetting(document, parent, "RaceViewer", settings.RaceViewer.Name);

            SettingsHelper.CreateSetting(document, parent, "AgreedToSRLRules", settings.AgreedToSRLRules);

            var recentSplits = document.CreateElement("RecentSplits");
            foreach (var splitsFile in settings.RecentSplits)
            {
                var splitsFileElement = SettingsHelper.ToElement(document, "SplitsFile", splitsFile.Path);
                splitsFileElement.SetAttribute("gameName", splitsFile.GameName);
                splitsFileElement.SetAttribute("categoryName", splitsFile.CategoryName);
                recentSplits.AppendChild(splitsFileElement);
            }
            parent.AppendChild(recentSplits);
            var recentLayouts = document.CreateElement("RecentLayouts");
            foreach (var layout in settings.RecentLayouts)
            {
                SettingsHelper.CreateSetting(document, recentLayouts, "LayoutPath", layout); 
            }
            parent.AppendChild(recentLayouts);

            SettingsHelper.CreateSetting(document, parent, "LastComparison", settings.LastComparison);
            SettingsHelper.CreateSetting(document, parent, "LastTimingMethod", settings.LastTimingMethod);
            SettingsHelper.CreateSetting(document, parent, "SimpleSumOfBest", settings.SimpleSumOfBest);

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
                SettingsHelper.CreateSetting(document, autoSplittersActive, "AutoSplitter", splitter);
            }
            parent.AppendChild(autoSplittersActive);

            SettingsHelper.CreateSetting(document, parent, "Drift", TimeStamp.NewDrift);

            document.Save(stream);
        }
    }
}
