using LiveSplit.UI;
using System;
using System.Drawing;
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

            parent.AppendChild(SettingsHelper.CreateImageElement(document, "GameIcon", run.GameIcon));
            parent.AppendChild(SettingsHelper.ToElement(document, "GameName", run.GameName));
            parent.AppendChild(SettingsHelper.ToElement(document, "CategoryName", run.CategoryName));
            parent.AppendChild(SettingsHelper.ToElement(document, "Offset", run.Offset));
            parent.AppendChild(SettingsHelper.ToElement(document, "AttemptCount", run.AttemptCount));

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

                splitElement.AppendChild(SettingsHelper.ToElement(document, "Name", segment.Name));
                splitElement.AppendChild(SettingsHelper.CreateImageElement(document, "Icon", segment.Icon));

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
                    history.AppendChild(historySegment.ToXml(document));
                }
                splitElement.AppendChild(history);
            }

            var autoSplitterSettings = document.CreateElement("AutoSplitterSettings");
            if (run.IsAutoSplitterActive())
                autoSplitterSettings.InnerXml = run.AutoSplitter.Component.GetSettings(document).InnerXml;
            parent.AppendChild(autoSplitterSettings);

            var metadata = document.CreateElement("Metadata");
            var platform = document.CreateElement("Platform");
            platform.Attributes.Append(SettingsHelper.ToAttribute(document, "id", run.Metadata.PlatformID ?? string.Empty));
            metadata.AppendChild(platform);
            var region = document.CreateElement("Region");
            region.Attributes.Append(SettingsHelper.ToAttribute(document, "id", run.Metadata.RegionID ?? string.Empty));
            metadata.AppendChild(region);
            var variables = document.CreateElement("Variables");
            foreach (var variable in run.Metadata.VariableValueIDs)
            {
                var variableElement = document.CreateElement("Variable");
                variableElement.Attributes.Append(SettingsHelper.ToAttribute(document, "id", variable.Key ?? string.Empty));
                variableElement.Attributes.Append(SettingsHelper.ToAttribute(document, "valueId", variable.Value ?? string.Empty));
                variables.AppendChild(variableElement);
            }
            metadata.AppendChild(variables);
            parent.AppendChild(metadata);

            document.Save(stream);
        }
    }
}
