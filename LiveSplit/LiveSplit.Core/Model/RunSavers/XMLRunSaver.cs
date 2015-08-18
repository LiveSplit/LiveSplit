using LiveSplit.UI;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace LiveSplit.Model.RunSavers
{
    public class XMLRunSaver : IRunSaver
    {
        public void Save(IRun run, Stream stream)
        {
            var document = new XmlDocument();

            XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(docNode);

            var parent = document.CreateElement("Run");
            parent.Attributes.Append(SettingsHelper.ToAttribute(document, "version", "1.6.0"));
            document.AppendChild(parent);

            SettingsHelper.CreateSetting(document, parent, "GameIcon", run.GameIcon);
            SettingsHelper.CreateSetting(document, parent, "GameName", run.GameName);
            SettingsHelper.CreateSetting(document, parent, "CategoryName", run.CategoryName);
            SettingsHelper.CreateSetting(document, parent, "Offset", run.Offset);
            SettingsHelper.CreateSetting(document, parent, "AttemptCount", run.AttemptCount);

            var runHistory = document.CreateElement("AttemptHistory");
            foreach (var attempt in run.AttemptHistory)
            {
                runHistory.AppendChild(attempt.ToXml(document));
            }
            parent.AppendChild(runHistory);

            var segmentElement = document.CreateElement("Segments");
            parent.AppendChild(segmentElement);

            var bf = new BinaryFormatter();

            foreach (var segment in run)
            {
                var splitElement = document.CreateElement("Segment");
                segmentElement.AppendChild(splitElement);

                SettingsHelper.CreateSetting(document, splitElement, "Name", segment.Name);
                SettingsHelper.CreateSetting(document, splitElement, "Icon", segment.Icon);

                var splitTimes = document.CreateElement("SplitTimes");
                foreach (var comparison in run.CustomComparisons)
                {
                    var splitTime = segment.Comparisons[comparison].ToXml(document, "SplitTime");
                    splitTime.Attributes.Append(SettingsHelper.ToAttribute(document, "name", comparison));
                    splitTimes.AppendChild(splitTime);
                }
                splitElement.AppendChild(splitTimes);

                splitElement.AppendChild(segment.BestSegmentTime.ToXml(document, "BestSegmentTime"));

                var history = document.CreateElement("SegmentHistory");
                foreach (var historySegment in segment.SegmentHistory)
                {
                    var indexedTime = new IndexedTime(historySegment.Value, historySegment.Key);
                    history.AppendChild(indexedTime.ToXml(document));
                }
                splitElement.AppendChild(history);
            }

            var autoSplitterSettings = document.CreateElement("AutoSplitterSettings");
            if (run.IsAutoSplitterActive())
                autoSplitterSettings.InnerXml = run.AutoSplitter.Component.GetSettings(document).InnerXml;
            parent.AppendChild(autoSplitterSettings);

            var metadata = document.CreateElement("Metadata");

            var runElement = document.CreateElement("Run");
            runElement.Attributes.Append(SettingsHelper.ToAttribute(document, "id", run.Metadata.RunID ?? string.Empty));
            metadata.AppendChild(runElement);

            var platform = SettingsHelper.ToElement(document, "Platform", run.Metadata.PlatformName ?? string.Empty);
            platform.Attributes.Append(SettingsHelper.ToAttribute(document, "usesEmulator", run.Metadata.UsesEmulator));
            metadata.AppendChild(platform);

            SettingsHelper.CreateSetting(document, metadata, "Region", run.Metadata.RegionName ?? string.Empty);

            var variables = document.CreateElement("Variables");
            foreach (var variable in run.Metadata.VariableValueNames)
            {
                var variableElement = SettingsHelper.ToElement(document, "Variable", variable.Value ?? string.Empty);
                variableElement.Attributes.Append(SettingsHelper.ToAttribute(document, "name", variable.Key ?? string.Empty));
                variables.AppendChild(variableElement);
            }
            metadata.AppendChild(variables);
            parent.AppendChild(metadata);

            document.Save(stream);
        }
    }
}
