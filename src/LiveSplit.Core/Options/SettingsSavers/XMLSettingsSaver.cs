using System;
using System.Xml;

using LiveSplit.Model;

using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Options.SettingsSavers;

public class XMLSettingsSaver : ISettingsSaver
{
    public void Save(ISettings settings, System.IO.Stream stream)
    {
        var document = new XmlDocument();

        XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
        document.AppendChild(docNode);

        XmlElement parent = document.CreateElement("Settings");
        XmlAttribute version = document.CreateAttribute("version");
        version.Value = "1.8.17";
        parent.Attributes.Append(version);
        document.AppendChild(parent);

        XmlElement hotkeyProfiles = document.CreateElement("HotkeyProfiles");
        foreach (System.Collections.Generic.KeyValuePair<string, HotkeyProfile> hotkeyProfile in settings.HotkeyProfiles)
        {
            XmlElement hotkeyProfileElement = hotkeyProfile.Value.ToXml(document);
            XmlAttribute name = document.CreateAttribute("name");
            name.Value = hotkeyProfile.Key;
            hotkeyProfileElement.Attributes.Append(name);
            hotkeyProfiles.AppendChild(hotkeyProfileElement);
        }

        parent.AppendChild(hotkeyProfiles);

        CreateSetting(document, parent, "WarnOnReset", settings.WarnOnReset);
        CreateSetting(document, parent, "RaceViewer", settings.RaceViewer.Name);
        CreateSetting(document, parent, "AgreedToSRLRules", settings.AgreedToSRLRules);

        XmlElement recentSplits = document.CreateElement("RecentSplits");
        foreach (RecentSplitsFile splitsFile in settings.RecentSplits)
        {
            XmlElement splitsFileElement = ToElement(document, "SplitsFile", splitsFile.Path);
            splitsFileElement.SetAttribute("gameName", splitsFile.GameName);
            splitsFileElement.SetAttribute("categoryName", splitsFile.CategoryName);
            splitsFileElement.SetAttribute("lastTimingMethod", splitsFile.LastTimingMethod.ToString());
            splitsFileElement.SetAttribute("lastHotkeyProfile", splitsFile.LastHotkeyProfile.ToString());
            recentSplits.AppendChild(splitsFileElement);
        }

        parent.AppendChild(recentSplits);
        XmlElement recentLayouts = document.CreateElement("RecentLayouts");
        foreach (string layout in settings.RecentLayouts)
        {
            CreateSetting(document, recentLayouts, "LayoutPath", layout);
        }

        parent.AppendChild(recentLayouts);

        CreateSetting(document, parent, "LastComparison", settings.LastComparison);
        CreateSetting(document, parent, "SimpleSumOfBest", settings.SimpleSumOfBest);
        CreateSetting(document, parent, "RefreshRate", settings.RefreshRate);
        CreateSetting(document, parent, "ServerPort", settings.ServerPort);

        XmlElement generatorStates = document.CreateElement("ComparisonGeneratorStates");
        foreach (System.Collections.Generic.KeyValuePair<string, bool> generator in settings.ComparisonGeneratorStates)
        {
            XmlElement generatorElement = document.CreateElement("Generator");
            XmlAttribute name = document.CreateAttribute("name");
            name.Value = generator.Key;
            generatorElement.Attributes.Append(name);
            generatorElement.InnerText = generator.Value.ToString();
            generatorStates.AppendChild(generatorElement);
        }

        parent.AppendChild(generatorStates);

        XmlElement raceProviderPlugins = document.CreateElement("RaceProviderPlugins");
        foreach (RaceProviderSettings raceProvider in settings.RaceProvider)
        {
            XmlElement raceProviderElement = raceProvider.ToXml(document);
            raceProviderPlugins.AppendChild(raceProviderElement);
        }

        parent.AppendChild(raceProviderPlugins);

        XmlElement autoSplittersActive = document.CreateElement("ActiveAutoSplitters");
        foreach (string splitter in settings.ActiveAutoSplitters)
        {
            CreateSetting(document, autoSplittersActive, "AutoSplitter", splitter);
        }

        parent.AppendChild(autoSplittersActive);

        AddDriftToSettings(document, parent);

        document.Save(stream);
    }

    public static void AddDriftToSettings(XmlDocument document, XmlElement parent)
    {
        XmlElement element = document.CreateElement("TimerDrift");
        byte[] data = BitConverter.GetBytes(TimeStamp.NewDrift);
        XmlCDataSection cdata = document.CreateCDataSection(Convert.ToBase64String(data));
        element.InnerXml = cdata.OuterXml;
        parent.AppendChild(element);
    }
}
