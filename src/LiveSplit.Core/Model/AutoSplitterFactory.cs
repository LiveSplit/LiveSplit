using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace LiveSplit.Model;

public class AutoSplitterFactory
{
    public static AutoSplitterFactory Instance { get; protected set; }
    public IDictionary<string, AutoSplitter> AutoSplitters { get; set; }

    public const string AutoSplittersXmlUrl = "https://cdn.jsdelivr.net/gh/LiveSplit/LiveSplit.AutoSplitters@master/LiveSplit.AutoSplitters.xml";
    public const string AutoSplittersXmlFile = "LiveSplit.AutoSplitters.xml";

    private static readonly TimeSpan RemoteTimeout = TimeSpan.FromSeconds(5);
    private readonly object _initLock = new();
    private bool _backgroundRefreshStarted;

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

        lock (_initLock)
        {
            if (AutoSplitters != null)
            {
                return;
            }

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            if (TryLoadFromFile(AutoSplittersXmlFile))
            {
                StartBackgroundRefresh();
                return;
            }

            StartBackgroundRefresh();
        }
    }

    private bool TryLoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return false;
            }

            var doc = new XmlDocument();
            doc.Load(path);
            var dict = Parse(doc);
            if (dict != null)
            {
                AutoSplitters = dict;
                return true;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return false;
    }

    private static IDictionary<string, AutoSplitter> Parse(XmlDocument document)
    {
        if (document == null || document["AutoSplitters"] == null)
        {
            return null;
        }

        try
        {
            return document["AutoSplitters"].ChildNodes.OfType<XmlElement>()
                .Where(element => element != null)
                .Select(CreateFromXmlElement)
                .SelectMany(x => x.Games.Select(y => new KeyValuePair<string, AutoSplitter>(y, x)))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    private void StartBackgroundRefresh()
    {
        if (_backgroundRefreshStarted)
        {
            return;
        }

        _backgroundRefreshStarted = true;

        Task.Run(async () =>
        {
            try
            {
                var doc = await DownloadRemoteAsync();
                if (doc == null)
                {
                    return;
                }

                try
                {
                    doc.Save(AutoSplittersXmlFile);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                var dict = Parse(doc);
                if (dict != null)
                {
                    lock (_initLock)
                    {
                        AutoSplitters = dict;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        });
    }

    private static async Task<XmlDocument> DownloadRemoteAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = RemoteTimeout };
            var xml = await client.GetStringAsync(AutoSplittersXmlUrl);
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
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
            autoSplitterType = scriptTypeElementText == "AutoSplittingRuntime" ? AutoSplitterType.AutoSplittingRuntimeScript : AutoSplitterType.Script;
        }

        return new AutoSplitter()
        {
            Description = element["Description"].InnerText,
            URLs = [.. element["URLs"].ChildNodes.OfType<XmlElement>().Select(x => x.InnerText)],
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
        try
        {
            var task = DownloadRemoteAsync();
            if (task.Wait(RemoteTimeout))
            {
                var doc = task.Result;
                if (doc != null)
                {
                    try
                    {
                        doc.Save(AutoSplittersXmlFile);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }

                    return doc;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        try
        {
            if (File.Exists(AutoSplittersXmlFile))
            {
                var doc = new XmlDocument();
                doc.Load(AutoSplittersXmlFile);
                return doc;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return null;
    }
}
