using LiveSplit.Model;
using System;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

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
            version.Value = "1.6.1";
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

            CreateSetting(document, parent, "GlobalHotkeysEnabled", settings.GlobalHotkeysEnabled);
            CreateSetting(document, parent, "DeactivateHotkeysForOtherPrograms", settings.DeactivateHotkeysForOtherPrograms);
            CreateSetting(document, parent, "WarnOnReset", settings.WarnOnReset);
            CreateSetting(document, parent, "DoubleTapPrevention", settings.DoubleTapPrevention);
            CreateSetting(document, parent, "HotkeyDelay", settings.HotkeyDelay);

            CreateSetting(document, parent, "RaceViewer", settings.RaceViewer.Name);

            CreateSetting(document, parent, "AgreedToSRLRules", settings.AgreedToSRLRules);

            var recentSplits = document.CreateElement("RecentSplits");
            foreach (var splitsFile in settings.RecentSplits)
            {
                var splitsFileElement = ToElement(document, "SplitsFile", splitsFile.Path);
                splitsFileElement.SetAttribute("gameName", splitsFile.GameName);
                splitsFileElement.SetAttribute("categoryName", splitsFile.CategoryName);
                splitsFileElement.SetAttribute("lastTimingMethod", splitsFile.LastTimingMethod.ToString());
                recentSplits.AppendChild(splitsFileElement);
            }
            parent.AppendChild(recentSplits);
            var recentLayouts = document.CreateElement("RecentLayouts");
            foreach (var layout in settings.RecentLayouts)
            {
                CreateSetting(document, recentLayouts, "LayoutPath", layout); 
            }
            parent.AppendChild(recentLayouts);

            CreateSetting(document, parent, "LastComparison", settings.LastComparison);
            CreateSetting(document, parent, "SimpleSumOfBest", settings.SimpleSumOfBest);

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
                CreateSetting(document, autoSplittersActive, "AutoSplitter", splitter);
            }
            parent.AppendChild(autoSplittersActive);

            AddDriftToSettings(document, parent);

            document.Save(stream);
        }

        public static void AddDriftToSettings(XmlDocument document, XmlElement parent)
        {
            var element = document.CreateElement("TimerDrift");
            var data = BitConverter.GetBytes(TimeStamp.NewDrift);
            var cdata = document.CreateCDataSection(Convert.ToBase64String(data));
            element.InnerXml = cdata.OuterXml;
            parent.AppendChild(element);
        }
    }

    
}
