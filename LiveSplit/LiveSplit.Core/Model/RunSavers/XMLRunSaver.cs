using LiveSplit.UI;
using System.IO;
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
            parent.Attributes.Append(SettingsHelper.ToAttribute(document, "version", "1.5.0"));
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

            document.Save(stream);
        }
    }
}
