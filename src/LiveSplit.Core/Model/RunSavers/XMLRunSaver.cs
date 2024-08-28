using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Model.RunSavers;

public class XMLRunSaver : IRunSaver
{
    public void Save(IRun run, Stream stream)
    {
        var document = new XmlDocument();

        XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
        document.AppendChild(docNode);

        XmlElement parent = document.CreateElement("Run");
        parent.Attributes.Append(ToAttribute(document, "version", "1.7.0"));
        document.AppendChild(parent);

        CreateSetting(document, parent, "GameIcon", run.GameIcon);
        CreateSetting(document, parent, "GameName", run.GameName);
        CreateSetting(document, parent, "CategoryName", run.CategoryName);
        CreateSetting(document, parent, "LayoutPath", run.LayoutPath);

        XmlElement metadata = document.CreateElement("Metadata");

        XmlElement runElement = document.CreateElement("Run");
        runElement.Attributes.Append(ToAttribute(document, "id", run.Metadata.RunID));
        metadata.AppendChild(runElement);

        XmlElement platform = ToElement(document, "Platform", run.Metadata.PlatformName);
        platform.Attributes.Append(ToAttribute(document, "usesEmulator", run.Metadata.UsesEmulator));
        metadata.AppendChild(platform);

        CreateSetting(document, metadata, "Region", run.Metadata.RegionName);

        XmlElement variables = document.CreateElement("Variables");
        foreach (System.Collections.Generic.KeyValuePair<string, string> variable in run.Metadata.VariableValueNames)
        {
            XmlElement variableElement = ToElement(document, "Variable", variable.Value);
            variableElement.Attributes.Append(ToAttribute(document, "name", variable.Key));
            variables.AppendChild(variableElement);
        }

        metadata.AppendChild(variables);

        XmlElement customVariables = document.CreateElement("CustomVariables");
        foreach (System.Collections.Generic.KeyValuePair<string, CustomVariable> entry in run.Metadata.CustomVariables)
        {
            if (entry.Value.IsPermanent)
            {
                XmlElement customVariableElement = ToElement(document, "Variable", entry.Value.Value);
                customVariableElement.Attributes.Append(ToAttribute(document, "name", entry.Key));
                customVariables.AppendChild(customVariableElement);
            }
        }

        metadata.AppendChild(customVariables);

        parent.AppendChild(metadata);

        CreateSetting(document, parent, "Offset", run.Offset);
        CreateSetting(document, parent, "AttemptCount", run.AttemptCount);

        XmlElement runHistory = document.CreateElement("AttemptHistory");
        foreach (Attempt attempt in run.AttemptHistory)
        {
            runHistory.AppendChild(attempt.ToXml(document));
        }

        parent.AppendChild(runHistory);

        XmlElement segmentElement = document.CreateElement("Segments");
        parent.AppendChild(segmentElement);

        var bf = new BinaryFormatter();

        foreach (ISegment segment in run)
        {
            XmlElement splitElement = document.CreateElement("Segment");
            segmentElement.AppendChild(splitElement);

            CreateSetting(document, splitElement, "Name", segment.Name);
            CreateSetting(document, splitElement, "Icon", segment.Icon);

            XmlElement splitTimes = document.CreateElement("SplitTimes");
            foreach (string comparison in run.CustomComparisons)
            {
                XmlElement splitTime = segment.Comparisons[comparison].ToXml(document, "SplitTime");
                splitTime.Attributes.Append(ToAttribute(document, "name", comparison));
                splitTimes.AppendChild(splitTime);
            }

            splitElement.AppendChild(splitTimes);

            splitElement.AppendChild(segment.BestSegmentTime.ToXml(document, "BestSegmentTime"));

            XmlElement history = document.CreateElement("SegmentHistory");
            foreach (System.Collections.Generic.KeyValuePair<int, Time> historySegment in segment.SegmentHistory)
            {
                var indexedTime = new IndexedTime(historySegment.Value, historySegment.Key);
                history.AppendChild(indexedTime.ToXml(document));
            }

            splitElement.AppendChild(history);
        }

        XmlElement autoSplitterSettings = document.CreateElement("AutoSplitterSettings");
        if (run.IsAutoSplitterActive())
        {
            autoSplitterSettings.InnerXml = run.AutoSplitter.Component.GetSettings(document).InnerXml;
        }

        parent.AppendChild(autoSplitterSettings);

        document.Save(stream);
    }
}
