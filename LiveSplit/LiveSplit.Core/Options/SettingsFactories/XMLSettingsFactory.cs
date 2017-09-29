using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.Input;
using LiveSplit.Model.RunFactories;
using LiveSplit.Web.SRL;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Options.SettingsFactories
{
    public class XMLSettingsFactory : ISettingsFactory
    {
        public Stream Stream { get; set; }

        public XMLSettingsFactory(Stream stream)
        {
            Stream = stream;
        }

        public ISettings Create()
        {
            var document = new XmlDocument();
            document.Load(Stream);
            var settings = new StandardSettingsFactory().Create();

            var parent = document["Settings"];
            var version = ParseAttributeVersion(parent);

            settings.GlobalHotkeysEnabled = ParseBool(parent["GlobalHotkeysEnabled"]);
            settings.WarnOnReset = ParseBool(parent["WarnOnReset"], settings.WarnOnReset);
            settings.DoubleTapPrevention = ParseBool(parent["DoubleTapPrevention"], settings.DoubleTapPrevention);
            settings.SimpleSumOfBest = ParseBool(parent["SimpleSumOfBest"], settings.SimpleSumOfBest);
            settings.LastComparison = ParseString(parent["LastComparison"], settings.LastComparison);
            settings.DeactivateHotkeysForOtherPrograms = ParseBool(parent["DeactivateHotkeysForOtherPrograms"], settings.DeactivateHotkeysForOtherPrograms);
            settings.HotkeyDelay = ParseFloat(parent["HotkeyDelay"], settings.HotkeyDelay);
            settings.AgreedToSRLRules = ParseBool(parent["AgreedToSRLRules"], settings.AgreedToSRLRules);

            var recentLayouts = parent["RecentLayouts"];
            foreach (var layoutNode in recentLayouts.GetElementsByTagName("LayoutPath"))
            {
                var layoutElement = layoutNode as XmlElement;
                settings.RecentLayouts.Add(layoutElement.InnerText);
            }

            if (version >= new Version(1, 3))
            {
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
                foreach (var generatorNode in parent["ComparisonGeneratorStates"].ChildNodes.OfType<XmlElement>())
                {
                    var comparisonName = generatorNode.GetAttribute("name");
                    if (settings.ComparisonGeneratorStates.ContainsKey(comparisonName))
                        settings.ComparisonGeneratorStates[comparisonName] = bool.Parse(generatorNode.InnerText);
                }

                foreach (var splitNode in recentSplits.GetElementsByTagName("SplitsFile"))
                {
                    var splitElement = splitNode as XmlElement;
                    string gameName = splitElement.GetAttribute("gameName");
                    string categoryName = splitElement.GetAttribute("categoryName");
                    var method = TimingMethod.RealTime;
                    if (version >= new Version(1, 6, 1))
                        method = (TimingMethod)Enum.Parse(typeof(TimingMethod), splitElement.GetAttribute("lastTimingMethod"));
                    var path = splitElement.InnerText;

                    var recentSplitsFile = new RecentSplitsFile(path, method, gameName, categoryName);
                    settings.RecentSplits.Add(recentSplitsFile);
                }
            }
            else
            {
                var comparisonsFactory = new StandardComparisonGeneratorsFactory();
                var runFactory = new StandardFormatsRunFactory();

                foreach (var splitNode in recentSplits.GetElementsByTagName("SplitsPath"))
                {
                    var splitElement = splitNode as XmlElement;
                    var path = splitElement.InnerText;

                    try
                    {
                        using (var stream = File.OpenRead(path))
                        {
                            runFactory.FilePath = path;
                            runFactory.Stream = stream;
                            var run = runFactory.Create(comparisonsFactory);

                            var recentSplitsFile = new RecentSplitsFile(path, run, TimingMethod.RealTime);
                            settings.RecentSplits.Add(recentSplitsFile);
                        }
                    }
                    catch { }
                }
            }

            if (version >= new Version(1, 8))
            {
                foreach (var hotkeySetNode in parent["HotkeySets"].ChildNodes.OfType<XmlElement>())
                {
                    var hotkeySetName = hotkeySetNode.GetAttribute("name");
                    settings.HotkeySets.Remove(hotkeySetName);
                    settings.HotkeySets[hotkeySetName] = HotkeySet.FromXml(hotkeySetNode, version);
                }
            }
            else
            {
                var hotkeySet = HotkeySet.FromXml(parent, version);
                settings.HotkeySets["Default"] = hotkeySet;
            }

            LoadDrift(parent);

            return settings;
        }

        private static void LoadDrift(XmlElement parent)
        {
            var element = parent["TimerDrift"];
            if (element != null)
            {
                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);
                TimeStamp.PersistentDrift = TimeStamp.NewDrift = BitConverter.ToDouble(data, 0);
            }
        }
    }
}
