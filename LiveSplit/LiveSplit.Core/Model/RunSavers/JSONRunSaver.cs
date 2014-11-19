using LiveSplit.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LiveSplit.Model.RunSavers
{
    public class JSONRunSaver : IRunSaver
    {
        private String CreateImageElement(Image image)
        {
            if (image != null)
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter();

                    bf.Serialize(ms, image);
                    var data = ms.ToArray();
                    return Convert.ToBase64String(data);
                }
            }

            return "";
        }

        public void Save(IRun run, Stream stream)
        {
            dynamic document = new DynamicJsonObject();

            document.version = "1.4";
            document.gameIcon = CreateImageElement(run.GameIcon);
            document.gameName = run.GameName;
            document.categoryName = run.CategoryName;
            document.offset = run.Offset.ToString();
            document.attemptCount = run.AttemptCount.ToString();

            var runHistory = new List<DynamicJsonObject>();
            document.runHistory = runHistory;
            foreach (var historyItem in run.RunHistory)
            {
                runHistory.Add(historyItem.ToJson());
            }

            var segments = new List<DynamicJsonObject>();
            document.segments = segments;
            foreach(var segment in run)
            {
                dynamic segmentElement = new DynamicJsonObject();
                segmentElement.name = segment.Name;
                segmentElement.icon = CreateImageElement(segment.Icon);

                dynamic splitTimes = new DynamicJsonObject();
                foreach (var comparison in run.CustomComparisons)
                    splitTimes.Properties.Add(comparison, segment.Comparisons[comparison].ToJson());
                segmentElement.splitTimes = splitTimes;

                segments.Add(segmentElement);
            }

            /*
            var segmentElement = document.CreateElement("Segments");
            parent.AppendChild(segmentElement);

            var bf = new BinaryFormatter();

            foreach (var segment in run)
            {
                var splitElement = document.CreateElement("Segment");
                segmentElement.AppendChild(splitElement);

                var icon = CreateImageElement(document, "Icon", segment.Icon);
                splitElement.AppendChild(icon);

                var name = document.CreateElement("Name");
                name.InnerText = segment.Name;
                splitElement.AppendChild(name);

                var splitTimes = document.CreateElement("SplitTimes");
                foreach (var comparison in run.CustomComparisons)
                {
                    var splitTime = document.CreateElement("SplitTime");
                    var comparisonName = document.CreateAttribute("name");
                    comparisonName.Value = comparison;
                    splitTime.Attributes.Append(comparisonName);
                    splitTime.InnerText = segment.Comparisons[comparison].ToString();
                    splitTimes.AppendChild(splitTime);
                }
                splitElement.AppendChild(splitTimes);

                var goldSplit = document.CreateElement("BestSegmentTime");
                goldSplit.InnerText = segment.BestSegmentTime.ToString();
                splitElement.AppendChild(goldSplit);

                var history = document.CreateElement("SegmentHistory");
                foreach (var historySegment in segment.SegmentHistory)
                {
                    history.AppendChild(historySegment.ToXml(document));
                }
                splitElement.AppendChild(history);
            }*/

            var writer = new StreamWriter(stream);
            writer.Write(document.ToString());
            writer.Flush();
        }
    }
}
