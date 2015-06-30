using LiveSplit.Model.Comparisons;
using LiveSplit.UI;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Xml;

namespace LiveSplit.Model.RunFactories
{
    public class XMLRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }
        public string FilePath { get; set; }

        public XMLRunFactory(Stream stream = null, string filePath = null)
        {
            Stream = stream;
            FilePath = filePath;
        }

        private void ParseAttemptHistory(Version version, XmlElement parent, IRun run)
        {
            if (version >= new Version(1, 5, 0))
            {
                var attemptHistory = parent["AttemptHistory"];
                foreach (var attemptNode in attemptHistory.GetElementsByTagName("Attempt"))
                {
                    var attempt = Attempt.ParseXml(attemptNode as XmlElement);
                    run.AttemptHistory.Add(attempt);
                }
            }
            else if (version >= new Version(1, 4, 1))
            {
                var runHistory = parent["RunHistory"];
                foreach (var runHistoryNode in runHistory.GetElementsByTagName("Time"))
                {
                    var indexedTime = IndexedTimeHelper.ParseXml(runHistoryNode as XmlElement);
                    var attempt = new Attempt(indexedTime.Index, indexedTime.Time, null, null);
                    run.AttemptHistory.Add(attempt);
                }
            }
            else
            {
                var runHistory = parent["RunHistory"];
                foreach (var runHistoryNode in runHistory.GetElementsByTagName("Time"))
                {
                    var indexedTime = IndexedTimeHelper.ParseXmlOld(runHistoryNode as XmlElement);
                    var attempt = new Attempt(indexedTime.Index, indexedTime.Time, null, null);
                    run.AttemptHistory.Add(attempt);
                }
            }
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var document = new XmlDocument();
            document.Load(Stream);

            var run = new Run(factory);
            var parent = document["Run"];
            var version = SettingsHelper.ParseAttributeVersion(parent);

            run.GameIcon = SettingsHelper.GetImageFromElement(parent["GameIcon"]);
            run.GameName = SettingsHelper.ParseString(parent["GameName"]);
            run.CategoryName = SettingsHelper.ParseString(parent["CategoryName"]);
            run.Offset = SettingsHelper.ParseTimeSpan(parent["Offset"]);
            run.AttemptCount = SettingsHelper.ParseInt(parent["AttemptCount"]);

            ParseAttemptHistory(version, parent, run);

            var segmentsNode = parent["Segments"];

            foreach (var segmentNode in segmentsNode.GetElementsByTagName("Segment"))
            {
                var segmentElement = segmentNode as XmlElement;

                var split = new Segment(SettingsHelper.ParseString(segmentElement["Name"]));
                split.Icon = SettingsHelper.GetImageFromElement(segmentElement["Icon"]);

                if (version >= new Version(1, 3))
                {
                    var splitTimes = segmentElement["SplitTimes"];
                    foreach (var comparisonNode in splitTimes.GetElementsByTagName("SplitTime"))
                    {
                        var comparisonElement = comparisonNode as XmlElement;
                        var comparisonName = comparisonElement.GetAttribute("name");
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
                var newXmlDoc = new XmlDocument();
                newXmlDoc.InnerXml = parent["AutoSplitterSettings"].OuterXml;
                run.AutoSplitterSettings = newXmlDoc.FirstChild as XmlElement;
                run.AutoSplitterSettings.Attributes.Append(SettingsHelper.ToAttribute(newXmlDoc, "gameName", run.GameName));
            }

            if (version >= new Version(1, 6))
            {
                var metadata = parent["Metadata"];
                run.Metadata.PlatformID = metadata["Platform"].GetAttribute("id");
                run.Metadata.RegionID = metadata["Region"].GetAttribute("id");
                foreach (var variableNode in metadata["Variables"].ChildNodes.OfType<XmlElement>())
                {
                    run.Metadata.VariableValueIDs.Add(variableNode.GetAttribute("id"), variableNode.GetAttribute("valueId"));
                }
            }

            if (!string.IsNullOrEmpty(FilePath))
                run.FilePath = FilePath;

            return run;
        }
    }
}
