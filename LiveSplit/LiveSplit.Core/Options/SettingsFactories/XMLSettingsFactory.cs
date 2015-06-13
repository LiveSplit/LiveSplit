using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.UI;
using LiveSplit.Web.SRL;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace LiveSplit.Options.SettingsFactories
{
    public class XMLSettingsFactory : ISettingsFactory
    {
        public Stream Stream { get; set; }
 
        public XMLSettingsFactory (Stream stream)
        {
            Stream = stream;
        }

        public ISettings Create()
        {
            var document = new XmlDocument();
            document.Load(Stream);
            var settings = new StandardSettingsFactory().Create();

            var parent = document["Settings"];
            var version = parent.HasAttribute("version") 
                ? Version.Parse(parent.Attributes["version"].Value) 
                : new Version(1,0,0,0);

            var keyStart = parent["SplitKey"];
            if (!string.IsNullOrEmpty(keyStart.InnerText))
                settings.SplitKey = new KeyOrButton(keyStart.InnerText);
            else
                settings.SplitKey = null;

            var keyReset = parent["ResetKey"];
            if (!string.IsNullOrEmpty(keyReset.InnerText))
                settings.ResetKey = new KeyOrButton(keyReset.InnerText);
            else
                settings.ResetKey = null;

            var keySkip = parent["SkipKey"];
            if (!string.IsNullOrEmpty(keySkip.InnerText))
                settings.SkipKey = new KeyOrButton(keySkip.InnerText);
            else
                settings.SkipKey = null;

            var keyUndo = parent["UndoKey"];
            if (!string.IsNullOrEmpty(keyUndo.InnerText))
                settings.UndoKey = new KeyOrButton(keyUndo.InnerText);
            else
                settings.UndoKey = null;

            settings.GlobalHotkeysEnabled = SettingsHelper.ParseBool(parent["GlobalHotkeysEnabled"]);
            settings.WarnOnReset = SettingsHelper.ParseBool(parent["WarnOnReset"], settings.WarnOnReset);
            settings.DoubleTapPrevention = SettingsHelper.ParseBool(parent["DoubleTapPrevention"], settings.DoubleTapPrevention);
            settings.LastTimingMethod = SettingsHelper.ParseEnum<TimingMethod>(parent["LastTimingMethod"], settings.LastTimingMethod);
            settings.SimpleSumOfBest = SettingsHelper.ParseBool(parent["SimpleSumOfBest"], settings.SimpleSumOfBest);
            settings.LastComparison = SettingsHelper.ParseString(parent["LastComparison"], settings.LastComparison);
            settings.DeactivateHotkeysForOtherPrograms = SettingsHelper.ParseBool(parent["DeactivateHotkeysForOtherPrograms"], settings.DeactivateHotkeysForOtherPrograms);
            settings.HotkeyDelay = SettingsHelper.ParseFloat(parent["HotkeyDelay"], settings.HotkeyDelay);
            settings.AgreedToSRLRules = SettingsHelper.ParseBool(parent["AgreedToSRLRules"], settings.AgreedToSRLRules);

            var recentLayouts = parent["RecentLayouts"];
            foreach (var layoutNode in recentLayouts.GetElementsByTagName("LayoutPath"))
            {
                var layoutElement = layoutNode as XmlElement;
                settings.RecentLayouts.Add(layoutElement.InnerText);
            }

            if (version > new Version(1, 0, 0, 0))
            {
                var keyPause = parent["PauseKey"];
                if (!string.IsNullOrEmpty(keyPause.InnerText))
                    settings.PauseKey = new KeyOrButton(keyPause.InnerText);
                else
                    settings.PauseKey = null;

                var keyToggle = parent["ToggleGlobalHotkeys"];
                if (!string.IsNullOrEmpty(keyToggle.InnerText))
                    settings.ToggleGlobalHotkeys = new KeyOrButton(keyToggle.InnerText);
                else
                    settings.ToggleGlobalHotkeys = null;
            }

            if (version >= new Version(1, 3))
            {
                var switchComparisonPrevious = parent["SwitchComparisonPrevious"];
                if (!string.IsNullOrEmpty(switchComparisonPrevious.InnerText))
                    settings.SwitchComparisonPrevious = new KeyOrButton(switchComparisonPrevious.InnerText);
                else
                    settings.SwitchComparisonPrevious = null;
                var switchComparisonNext = parent["SwitchComparisonNext"];
                if (!string.IsNullOrEmpty(switchComparisonNext.InnerText))
                    settings.SwitchComparisonNext = new KeyOrButton(switchComparisonNext.InnerText);
                else
                    settings.SwitchComparisonNext = null;

                settings.RaceViewer = RaceViewer.FromName(parent["RaceViewer"].InnerText);
            }

            if (version >= new Version(1, 4))
            {
                var activeAutoSplitters = parent["ActiveAutoSplitters"];
                foreach (var splitter in activeAutoSplitters.GetElementsByTagName("AutoSplitter").OfType<XmlElement>())
                {
                    settings.ActiveAutoSplitters.Add(splitter.InnerText);
                }
            }

            var recentSplits = parent["RecentSplits"];

            if (version >= new Version(1, 6))
            {
                foreach (var generatorNode in parent["ComparisonGeneratorStates"].ChildNodes)
                {
                    settings.ComparisonGeneratorStates[((XmlNode)generatorNode).Attributes["name"].Value] = Boolean.Parse(((XmlNode)generatorNode).InnerText);
                }

                foreach (var splitNode in recentSplits.GetElementsByTagName("SplitsFile"))
                {
                    var splitElement = splitNode as XmlElement;
                    string gameName = splitElement.GetAttribute("gameName");
                    string categoryName = splitElement.GetAttribute("categoryName");
                    var path = splitElement.InnerText;

                    var recentSplitsFile = new RecentSplitsFile(path, gameName, categoryName);
                    settings.RecentSplits.Add(recentSplitsFile);
                }
            }
            else
            {
                foreach (var splitNode in recentSplits.GetElementsByTagName("SplitsPath"))
                {
                    var splitElement = splitNode as XmlElement;
                    var path = splitElement.InnerText;

                    var recentSplitsFile = new RecentSplitsFile(path);
                    settings.RecentSplits.Add(recentSplitsFile);
                }
            }

            return settings;
        }
    }
}
