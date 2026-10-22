using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Xunit;

namespace LiveSplit.Tests.AutoSplitterTests;

public class AutoSplitterXML
{
    [Fact]
    public void TestAutoSplittersXML()
    {
        string xmlPath = Path.Combine("..", "..", "..", "..", AutoSplitterFactory.AutoSplittersXmlFile);

        // Only download the latest XML if the file does not exist. This allows overriding the file
        // for testing a specific XML.
        if (!File.Exists(xmlPath))
        {
            using var client = new WebClient();
            client.DownloadFile(AutoSplitterFactory.AutoSplittersXmlUrl, xmlPath);
        }

        Assert.True(File.Exists(xmlPath), "The Auto Splitters XML is missing");

        var document = new XmlDocument();
        document.Load(xmlPath);

        IEnumerable<XmlElement> autoSplitterElems = document["AutoSplitters"].ChildNodes.OfType<XmlElement>().Where(element => element != null);
        Assert.True(autoSplitterElems.All(x => x.Name == "AutoSplitter"), "<AutoSplitters> must contain only <AutoSplitter> elements");

        IEnumerable<XmlElement> gameElems = autoSplitterElems.SelectMany(x => x["Games"].ChildNodes.OfType<XmlElement>());
        Assert.True(gameElems.All(x => x.Name == "Game"), "<Games> must contain only <Game> elements");

        IEnumerable<XmlElement> urlElems = autoSplitterElems.SelectMany(x => x["URLs"].ChildNodes.OfType<XmlElement>());
        Assert.True(urlElems.All(x => x.Name == "URL"), "<URLs> must contain only <URL> elements");

        IDictionary<string, AutoSplitter> autoSplitters = autoSplitterElems
            .Select(AutoSplitterFactory.CreateFromXmlElement)
            .SelectMany(x => x.Games.Select(y => new KeyValuePair<string, AutoSplitter>(y, x)))
            .ToDictionary(x => x.Key, x => x.Value);

        Assert.True(!autoSplitters.Any(x => string.IsNullOrWhiteSpace(x.Key)), "Empty Game Names are not allowed");
        Assert.True(!autoSplitters.Values.Any(x => string.IsNullOrWhiteSpace(x.Description)), "Auto Splitters need a description");
        Assert.True(!autoSplitters.Values.Any(x => x.Description.Length > 120), "Auto Splitter description must be no longer than 120 characters");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Count == 0), "Auto Splitters need to have at least one URL");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => y.EndsWith(".asl")) && x.Type == AutoSplitterType.Component),
            "ASL Script is downloaded even though Type \"Component\" is specified");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => y.EndsWith(".asl")) && x.Type == AutoSplitterType.AutoSplittingRuntimeScript),
            "ASL Script is downloaded even though ScriptType \"AutoSplittingRuntime\" is specified");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs[0].EndsWith(".dll") && (x.Type == AutoSplitterType.Script || x.Type == AutoSplitterType.AutoSplittingRuntimeScript)),
            "Component is downloaded even though Type \"Script\" is specified");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => y.EndsWith(".wasm")) && x.Type == AutoSplitterType.Component),
            "WebAssembly Script is downloaded even though Type \"Component\" is specified");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => y.EndsWith(".wasm")) && x.Type == AutoSplitterType.Script),
            "WebAssembly Script is downloaded even though ScriptType \"AutoSplittingRuntime\" is not specified");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => !Uri.IsWellFormedUriString(y, UriKind.Absolute))),
            "Auto Splitters need to have valid URLs");
        Assert.True(!autoSplitters.Values.Any(x => x.URLs.Any(y => Regex.IsMatch(y, "https://github.com/[^/]*/[^/]*/blob/"))),
            "URLs leading to GitHub should use the raw file link");

        // Test parsing with a fixed example so this does not depend on the downloaded XML.
        var runtimeDocument = new XmlDocument();
        runtimeDocument.LoadXml("""
            <AutoSplitter>
                <Games>
                    <Game>Example Game</Game>
                </Games>
                <URLs>
                    <URL>https://example.com/example.asl</URL>
                </URLs>
                <Type>Script</Type>
                <Description>ASL splitter</Description>
                <Website>https://example.com/asl</Website>
                <AutoSplittingRuntime>
                    <URL>https://example.com/example.wasm</URL>
                    <Description>WASM splitter</Description>
                    <Website>https://example.com/wasm</Website>
                </AutoSplittingRuntime>
            </AutoSplitter>
            """);

        AutoSplitter runtimeAutoSplitter = AutoSplitterFactory.CreateFromXmlElement(runtimeDocument.DocumentElement);

        Assert.Equal(AutoSplitterType.Script, runtimeAutoSplitter.Type);
        Assert.Equal("https://example.com/example.asl", runtimeAutoSplitter.URLs.Single());
        Assert.NotNull(runtimeAutoSplitter.AutoSplittingRuntime);
        Assert.Equal("https://example.com/example.wasm", runtimeAutoSplitter.AutoSplittingRuntime.URL);
        Assert.Equal("WASM splitter", runtimeAutoSplitter.AutoSplittingRuntime.Description);
        Assert.Equal("https://example.com/wasm", runtimeAutoSplitter.AutoSplittingRuntime.Website);

        AutoSplitter runtimeClone = runtimeAutoSplitter.Clone();
        Assert.NotSame(runtimeAutoSplitter.AutoSplittingRuntime, runtimeClone.AutoSplittingRuntime);
        Assert.Equal(runtimeAutoSplitter.AutoSplittingRuntime.URL, runtimeClone.AutoSplittingRuntime.URL);
        Assert.Equal(runtimeAutoSplitter.AutoSplittingRuntime.Description, runtimeClone.AutoSplittingRuntime.Description);
        Assert.Equal(runtimeAutoSplitter.AutoSplittingRuntime.Website, runtimeClone.AutoSplittingRuntime.Website);

        // Validate AutoSplittingRuntime children in the downloaded XML.
        Assert.True(!autoSplitters.Values.Any(x => x.AutoSplittingRuntime != null && string.IsNullOrWhiteSpace(x.AutoSplittingRuntime.URL)),
            "AutoSplittingRuntime needs a URL");
        Assert.True(!autoSplitters.Values.Any(x => x.AutoSplittingRuntime != null && !Uri.IsWellFormedUriString(x.AutoSplittingRuntime.URL, UriKind.Absolute)),
            "AutoSplittingRuntime needs a valid URL");
        Assert.True(!autoSplitters.Values.Any(x => x.AutoSplittingRuntime != null && Regex.IsMatch(x.AutoSplittingRuntime.URL, "https://github.com/[^/]*/[^/]*/blob/")),
            "AutoSplittingRuntime URLs leading to GitHub should use the raw file link");
        Assert.True(!autoSplitters.Values.Any(x => x.AutoSplittingRuntime != null && string.IsNullOrWhiteSpace(x.AutoSplittingRuntime.Description)),
            "AutoSplittingRuntime needs a description");
        Assert.True(!autoSplitters.Values.Any(x => x.AutoSplittingRuntime != null && x.AutoSplittingRuntime.Description.Length > 120),
            "AutoSplittingRuntime description must be no longer than 120 characters");
        Assert.True(!autoSplitters.Values.Any(x => x.AutoSplittingRuntime != null && !x.AutoSplittingRuntime.URL.EndsWith(".wasm")),
            "AutoSplittingRuntime URL should point to a .wasm file");
    }
}
