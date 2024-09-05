using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

using LiveSplit.Options;

namespace LiveSplit.Model;

public class AutoSplitterFactory
{
    public static AutoSplitterFactory Instance { get; protected set; }
    public IDictionary<string, AutoSplitter> AutoSplitters { get; set; }

    public const string AutoSplittersXmlUrl = "https://raw.githubusercontent.com/LiveSplit/LiveSplit.AutoSplitters/master/LiveSplit.AutoSplitters.xml";
    public const string AutoSplittersXmlFile = "LiveSplit.AutoSplitters.xml";

    static AutoSplitterFactory()
    {
        try
        {
            Instance = new AutoSplitterFactory();
            Instance.Init();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    protected AutoSplitterFactory()
    {
    }

    public void Init()
    {
        if (AutoSplitters != null)
        {
            return;
        }

        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        XmlDocument document = DownloadAutoSplitters();

        if (document != null)
        {
            AutoSplitters = document["AutoSplitters"].ChildNodes.OfType<XmlElement>()
                .Where(element => element != null)
                .Select(CreateFromXmlElement)
                .SelectMany(x => x.Games.Select(y => new KeyValuePair<string, AutoSplitter>(y, x)))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public static AutoSplitter CreateFromXmlElement(XmlElement element)
    {
        string typeElementText = element["Type"]?.InnerText;
        string scriptTypeElementText = element["ScriptType"]?.InnerText;

        AutoSplitterType? autoSplitterType = null;
        if (typeElementText == "Component")
        {
            autoSplitterType = AutoSplitterType.Component;
        }
        else if (typeElementText == "Script")
        {
            if (scriptTypeElementText == "AutoSplittingRuntime")
            {
                autoSplitterType = AutoSplitterType.AutoSplittingRuntimeScript;
            }
            else
            {
                autoSplitterType = AutoSplitterType.Script;
            }
        }

        return new AutoSplitter()
        {
            Description = element["Description"].InnerText,
            URLs = element["URLs"].ChildNodes.OfType<XmlElement>().Select(x => x.InnerText).ToList(),
            Type = autoSplitterType.Value,
            Games = element["Games"].ChildNodes.OfType<XmlElement>().Select(x => (x.InnerText ?? "").ToLower()).ToList(),
            ShowInLayoutEditor = element["ShowInLayoutEditor"] != null,
            Website = element["Website"]?.InnerText
        };
    }

    public AutoSplitter Create(string game)
    {
        if (AutoSplitters == null)
        {
            Init();
        }

        if (AutoSplitters != null && !string.IsNullOrEmpty(game))
        {
            game = game.ToLower();

            if (AutoSplitters.ContainsKey(game))
            {
                return AutoSplitters[game];
            }
        }

        return null;
    }

    protected XmlDocument DownloadAutoSplitters()
    {
        var autoSplitters = new XmlDocument();
        try
        {
            autoSplitters.Load(AutoSplittersXmlUrl);
            autoSplitters.Save(AutoSplittersXmlFile);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            if (File.Exists(AutoSplittersXmlFile))
            {
                autoSplitters.Load(AutoSplittersXmlFile);
            }
            else
            {
                return null;
            }
        }

        return autoSplitters;
    }
}
