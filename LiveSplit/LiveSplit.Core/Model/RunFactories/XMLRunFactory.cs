using LiveSplit.Model.Comparisons;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace LiveSplit.Model.RunFactories
{
    public class XMLRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }
        public String FilePath { get; set; }

        public XMLRunFactory(Stream stream = null, String filePath = null)
        {
            Stream = stream;
            FilePath = filePath;
        }

        private static Image GetImageFromElement(XmlElement element)
        {
            if (!element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(data);
                return (Image)bf.Deserialize(ms);
            }
            return null;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var document = new XmlDocument();
            document.Load(Stream);

            var run = new Run(factory);

            var parent = document["Run"];

            var version = parent.HasAttribute("version")
                ? Version.Parse(parent.Attributes["version"].Value)
                : new Version(1, 0, 0, 0);

            run.GameIcon = GetImageFromElement(parent["GameIcon"]);
            run.GameName = parent["GameName"].InnerText;
            run.CategoryName = parent["CategoryName"].InnerText;
            run.Offset = TimeSpan.Parse(parent["Offset"].InnerText);
            run.AttemptCount = Int32.Parse(parent["AttemptCount"].InnerText);

            var runHistory = parent["RunHistory"];
            foreach (var runHistoryNode in runHistory.GetElementsByTagName("Time"))
            {
                run.RunHistory.Add(version >= new Version(1, 4, 1) ? IndexedTimeHelper.ParseXml(runHistoryNode as XmlElement) : IndexedTimeHelper.ParseXmlOld(runHistoryNode as XmlElement));
            }

            var segmentsNode = parent["Segments"];

            foreach (var segmentNode in segmentsNode.GetElementsByTagName("Segment"))
            {
                var segmentElement = segmentNode as XmlElement;

                var nameElement = segmentElement["Name"];
                var split = new Segment(nameElement.InnerText);

                var iconElement = segmentElement["Icon"];
                split.Icon = GetImageFromElement(iconElement);

                if (version >= new Version(1, 3))
                {
                    var splitTimes = segmentElement["SplitTimes"];
                    foreach (var comparisonNode in splitTimes.GetElementsByTagName("SplitTime"))
                    {
                        var comparisonElement = comparisonNode as XmlElement;
                        var comparisonName = comparisonElement.Attributes["name"].InnerText;
                        if (comparisonElement.InnerText.Length > 0)
                        {
                            split.Comparisons[comparisonName] = version >= new Version(1, 4, 1) ? Time.FromXml(comparisonElement) : Time.ParseText(comparisonElement.InnerText);
                        }
                        if (!run.CustomComparisons.Contains(comparisonName))
                            run.CustomComparisons.Add(comparisonName);
                    }
                }
                else
                {
                    var pbSplit = segmentElement["PersonalBestSplitTime"];
                    if (pbSplit.InnerText.Length > 0)
                    {
                        split.Comparisons[Run.PersonalBestComparisonName] = version >= new Version(1, 4, 1) ? Time.FromXml(pbSplit) : Time.ParseText(pbSplit.InnerText);
                    }
                }

                var goldSplit = segmentElement["BestSegmentTime"];
                if (goldSplit.InnerText.Length > 0)
                {
                    split.BestSegmentTime = version >= new Version(1, 4, 1) ? Time.FromXml(goldSplit) : Time.ParseText(goldSplit.InnerText);
                }

                var history = segmentElement["SegmentHistory"];
                foreach (var historyNode in history.GetElementsByTagName("Time"))
                {
                    split.SegmentHistory.Add(version >= new Version(1, 4, 1) ? IndexedTimeHelper.ParseXml(historyNode as XmlElement) : IndexedTimeHelper.ParseXmlOld(historyNode as XmlElement));
                }

                run.Add(split);
            }

            if (version >= new Version(1, 4, 2))
            {
                run.AutoSplitterSettings = parent["AutoSplitterSettings"];
                var gameName = document.CreateAttribute("gameName");
                gameName.Value = run.GameName;
                run.AutoSplitterSettings.Attributes.Append(gameName);
            }

            if (!String.IsNullOrEmpty(FilePath))
                run.FilePath = FilePath;

            return run;
        }
    }
}
