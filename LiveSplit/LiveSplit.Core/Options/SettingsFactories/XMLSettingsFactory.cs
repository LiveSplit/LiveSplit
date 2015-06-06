using LiveSplit.Model;
using LiveSplit.Model.Input;
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

                settings.WarnOnReset = bool.Parse(parent["WarnOnReset"].InnerText);
            }

            if (version >= new Version(1, 2))
                settings.DoubleTapPrevention = bool.Parse(parent["DoubleTapPrevention"].InnerText);

            if (version >= new Version(1, 4))
            {
                settings.LastTimingMethod = ParseEnum<TimingMethod>(parent["LastTimingMethod"]);
                settings.SimpleSumOfBest = bool.Parse(parent["SimpleSumOfBest"].InnerText);

                var activeAutoSplitters = parent["ActiveAutoSplitters"];
                foreach (var splitter in activeAutoSplitters.GetElementsByTagName("AutoSplitter").OfType<XmlElement>())
                {
                    settings.ActiveAutoSplitters.Add(splitter.InnerText);
                }
            }

            if (version >= new Version(1, 3))
            {
                //settings.RefreshRate = Single.Parse(parent["RefreshRate"].InnerText);
                settings.LastComparison = parent["LastComparison"].InnerText;
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
                settings.HotkeyDelay = float.Parse(parent["HotkeyDelay"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);

                settings.RaceViewer = RaceViewer.FromName(parent["RaceViewer"].InnerText);

                var deactivateHotkeysForOtherPrograms = parent["DeactivateHotkeysForOtherPrograms"];
                settings.DeactivateHotkeysForOtherPrograms = bool.Parse(deactivateHotkeysForOtherPrograms.InnerText);
            }

            if (version >= new Version(1, 3, 1))
            {
                settings.AgreedToSRLRules = bool.Parse(parent["AgreedToSRLRules"].InnerText);
            }

            var hotkeysEnabled = parent["GlobalHotkeysEnabled"];
            settings.GlobalHotkeysEnabled = bool.Parse(hotkeysEnabled.InnerText);
            var recentSplits = parent["RecentSplits"];
            foreach (var splitNode in recentSplits.GetElementsByTagName("SplitsPath"))
            {
                var splitElement = splitNode as XmlElement;
                settings.RecentSplits.Add(splitElement.InnerText);
            }
            var recentLayouts = parent["RecentLayouts"];
            foreach (var layoutNode in recentLayouts.GetElementsByTagName("LayoutPath"))
            {
                var layoutElement = layoutNode as XmlElement;
                settings.RecentLayouts.Add(layoutElement.InnerText);
            }
            return settings;
        }

        private T ParseEnum<T>(XmlElement element)
        {
            return (T)Enum.Parse(typeof(T), element.InnerText);
        }
    }
}
