using System;
using System.IO;
using System.Linq;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.UI.Components;
using LiveSplit.Web.SRL;

using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Options.SettingsFactories;

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
        ISettings settings = new StandardSettingsFactory().Create();

        XmlElement parent = document["Settings"];
        Version version = ParseAttributeVersion(parent);

        settings.WarnOnReset = ParseBool(parent["WarnOnReset"], settings.WarnOnReset);
        settings.SimpleSumOfBest = ParseBool(parent["SimpleSumOfBest"], settings.SimpleSumOfBest);
        settings.RefreshRate = ParseInt(parent["RefreshRate"], settings.RefreshRate);
        settings.ServerPort = ParseInt(parent["ServerPort"], settings.ServerPort);
        settings.LastComparison = ParseString(parent["LastComparison"], settings.LastComparison);
        settings.AgreedToSRLRules = ParseBool(parent["AgreedToSRLRules"], settings.AgreedToSRLRules);

        XmlElement recentLayouts = parent["RecentLayouts"];
        foreach (object layoutNode in recentLayouts.GetElementsByTagName("LayoutPath"))
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
            XmlElement activeAutoSplitters = parent["ActiveAutoSplitters"];
            foreach (XmlElement splitter in activeAutoSplitters.GetElementsByTagName("AutoSplitter").OfType<XmlElement>())
            {
                settings.ActiveAutoSplitters.Add(splitter.InnerText);
            }
        }

        XmlElement recentSplits = parent["RecentSplits"];

        if (version >= new Version(1, 6))
        {
            foreach (XmlElement generatorNode in parent["ComparisonGeneratorStates"].ChildNodes.OfType<XmlElement>())
            {
                string comparisonName = generatorNode.GetAttribute("name");
                if (settings.ComparisonGeneratorStates.ContainsKey(comparisonName))
                {
                    settings.ComparisonGeneratorStates[comparisonName] = bool.Parse(generatorNode.InnerText);
                }
            }

            foreach (object splitNode in recentSplits.GetElementsByTagName("SplitsFile"))
            {
                var splitElement = splitNode as XmlElement;
                string gameName = splitElement.GetAttribute("gameName");
                string categoryName = splitElement.GetAttribute("categoryName");

                TimingMethod method = TimingMethod.RealTime;
                if (version >= new Version(1, 6, 1))
                {
                    method = (TimingMethod)Enum.Parse(typeof(TimingMethod), splitElement.GetAttribute("lastTimingMethod"));
                }

                string hotkeyProfile = HotkeyProfile.DefaultHotkeyProfileName;
                if (version >= new Version(1, 8))
                {
                    hotkeyProfile = splitElement.GetAttribute("lastHotkeyProfile");
                }

                string path = splitElement.InnerText;

                var recentSplitsFile = new RecentSplitsFile(path, method, hotkeyProfile, gameName, categoryName);
                settings.RecentSplits.Add(recentSplitsFile);
            }
        }
        else
        {
            var comparisonsFactory = new StandardComparisonGeneratorsFactory();
            var runFactory = new StandardFormatsRunFactory();

            foreach (object splitNode in recentSplits.GetElementsByTagName("SplitsPath"))
            {
                var splitElement = splitNode as XmlElement;
                string path = splitElement.InnerText;

                try
                {
                    using FileStream stream = File.OpenRead(path);
                    runFactory.FilePath = path;
                    runFactory.Stream = stream;
                    IRun run = runFactory.Create(comparisonsFactory);

                    var recentSplitsFile = new RecentSplitsFile(path, run, TimingMethod.RealTime, HotkeyProfile.DefaultHotkeyProfileName);
                    settings.RecentSplits.Add(recentSplitsFile);
                }
                catch { }
            }
        }

        if (version >= new Version(1, 8))
        {
            settings.HotkeyProfiles.Clear();
            foreach (XmlElement hotkeyProfileNode in parent["HotkeyProfiles"].ChildNodes.OfType<XmlElement>())
            {
                string hotkeyProfileName = hotkeyProfileNode.GetAttribute("name");
                settings.HotkeyProfiles[hotkeyProfileName] = HotkeyProfile.FromXml(hotkeyProfileNode, version);
            }
        }
        else
        {
            var hotkeyProfile = HotkeyProfile.FromXml(parent, version);
            settings.HotkeyProfiles[HotkeyProfile.DefaultHotkeyProfileName] = hotkeyProfile;
        }

        settings.RaceProvider.Clear();
        if (version >= new Version(1, 8, 8))
        {
            foreach (XmlElement providerNode in parent["RaceProviderPlugins"].ChildNodes.OfType<XmlElement>())
            {
                string providerName = providerNode.GetAttribute("name");
                RaceProviderSettings raceProviderSettings = null;
                if (ComponentManager.RaceProviderFactories.ContainsKey(providerName))
                {
                    IRaceProviderFactory factory = ComponentManager.RaceProviderFactories[providerName];
                    raceProviderSettings = factory.CreateSettings();
                }
                else
                {
                    raceProviderSettings = new UnloadedRaceProviderSettings();
                }

                raceProviderSettings.FromXml(providerNode, version);
                settings.RaceProvider.Add(raceProviderSettings);
            }
        }

        foreach (IRaceProviderFactory factory in ComponentManager.RaceProviderFactories.Values)
        {
            RaceProviderSettings raceProviderSettings = factory.CreateSettings();
            if (!settings.RaceProvider.Any(x => x.GetType() == raceProviderSettings.GetType()))
            {
                settings.RaceProvider.Add(raceProviderSettings);
            }
        }

        LoadDrift(parent);

        return settings;
    }

    private static void LoadDrift(XmlElement parent)
    {
        XmlElement element = parent["TimerDrift"];
        if (element != null)
        {
            string base64String = element.InnerText;
            byte[] data = Convert.FromBase64String(base64String);
            double loadedDrift = BitConverter.ToDouble(data, 0);

            // Reset drift to 1 if it is too far off
            if (Math.Abs(loadedDrift - 1) > 0.01)
            {
                loadedDrift = 1;
            }

            TimeStamp.PersistentDrift = TimeStamp.NewDrift = loadedDrift;
        }
    }
}
