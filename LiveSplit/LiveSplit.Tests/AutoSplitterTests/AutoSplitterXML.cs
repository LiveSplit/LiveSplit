using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;
using LiveSplit.Model;

namespace LiveSplit.Tests.AutoSplitterTests
{
    public class AutoSplitterXML
    {
        [Fact]
        public void TestAutoSplittersXML()
        {
            var xmlPath = Path.Combine("..", "..", "..", "..", "LiveSplit.AutoSplitters.xml");

            Assert.True(File.Exists(xmlPath), "The Auto Splitters XML is missing");

            var document = new XmlDocument();
            document.Load(xmlPath);

            var autoSplitterElems = document["AutoSplitters"].ChildNodes.OfType<XmlElement>().Where(element => element != null);
            Assert.True(autoSplitterElems.All(x => x.Name == "AutoSplitter"), "<AutoSplitters> must contain only <AutoSplitter> elements");

            var gameElems = autoSplitterElems.SelectMany(x => x["Games"].ChildNodes.OfType<XmlElement>());
            Assert.True(gameElems.All(x => x.Name == "Game"), "<Games> must contain only <Game> elements");

            var urlElems = autoSplitterElems.SelectMany(x => x["URLs"].ChildNodes.OfType<XmlElement>());
            Assert.True(urlElems.All(x => x.Name == "URL"), "<URLs> must contain only <URL> elements");

            IDictionary<string, AutoSplitter> autoSplitters = autoSplitterElems.Select(element =>
                    new AutoSplitter()
                    {
                        Description = element["Description"].InnerText,
                        URLs = element["URLs"].ChildNodes.OfType<XmlElement>().Select(x => x.InnerText).ToList(),
                        Type = (AutoSplitterType)Enum.Parse(typeof(AutoSplitterType), element["Type"].InnerText),
                        Games = element["Games"].ChildNodes.OfType<XmlElement>().Select(x => (x.InnerText ?? "").ToLower()).ToList(),
                        ShowInLayoutEditor = element["ShowInLayoutEditor"] != null
                    }).SelectMany(x => x.Games.Select(y => new KeyValuePair<string, AutoSplitter>(y, x))).ToDictionary(x => x.Key, x => x.Value);

            Assert.True(!autoSplitters.Any(x => string.IsNullOrWhiteSpace(x.Key)), "Empty Game Names are not allowed");
            Assert.True(!autoSplitters.Values.Any(x => string.IsNullOrWhiteSpace(x.Description)), "Auto Splitters need a description");
            Assert.True(!autoSplitters.Values.Any(x => !x.URLs.Any()), "Auto Splitters need to have at least one URL");
            Assert.True(!autoSplitters.Values.Any(x => x.URLs.First().EndsWith(".asl")) && x.Type == AutoSplitterType.Component),
                "ASL Script is downloaded even though Type \"Component\" is specified");
            Assert.True(!autoSplitters.Values.Any(x => x.URLs.First().EndsWith(".dll")) && x.Type == AutoSplitterType.Script),
                "Component is downloaded even though Type \"Script\" is specified");
            Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => !Uri.IsWellFormedUriString(y, UriKind.Absolute))),
                "Auto Splitters need to have valid URLs");
            Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => Regex.IsMatch(y, "https://github.com/[^/]*/[^/]*/blob/"))),
                "URLs leading to GitHub should use the raw file link");
        }
    }
}
