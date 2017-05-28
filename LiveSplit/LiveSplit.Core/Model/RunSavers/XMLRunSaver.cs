using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

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
            parent.Attributes.Append(ToAttribute(document, "version", "1.7.0"));
            document.AppendChild(parent);

            CreateSetting(document, parent, "GameIcon", run.GameIcon);
            CreateSetting(document, parent, "GameName", run.GameName);
            CreateSetting(document, parent, "CategoryName", run.CategoryName);

            var metadata = document.CreateElement("Metadata");

            var runElement = document.CreateElement("Run");
            runElement.Attributes.Append(ToAttribute(document, "id", run.Metadata.RunID));
            metadata.AppendChild(runElement);

            var platform = ToElement(document, "Platform", run.Metadata.PlatformName);
            platform.Attributes.Append(ToAttribute(document, "usesEmulator", run.Metadata.UsesEmulator));
            metadata.AppendChild(platform);

            CreateSetting(document, metadata, "Region", run.Metadata.RegionName);

            var variables = document.CreateElement("Variables");
            foreach (var variable in run.Metadata.VariableValueNames)
            {
                var variableElement = ToElement(document, "Variable", variable.Value);
                variableElement.Attributes.Append(ToAttribute(document, "name", variable.Key));
                variables.AppendChild(variableElement);
            }
            metadata.AppendChild(variables);
            parent.AppendChild(metadata);

            CreateSetting(document, parent, "Offset", run.Offset);
            CreateSetting(document, parent, "AttemptCount", run.AttemptCount);

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

                CreateSetting(document, splitElement, "Name", segment.Name);
                CreateSetting(document, splitElement, "Icon", segment.Icon);

                var splitTimes = document.CreateElement("SplitTimes");
                foreach (var comparison in run.CustomComparisons)
                {
                    var splitTime = segment.Comparisons[comparison].ToXml(document, "SplitTime");
                    splitTime.Attributes.Append(ToAttribute(document, "name", comparison));
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

            document.Save(stream);
        }
    }
}
