using LiveSplit.Localization;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace LiveSplit.Model;

public class AutoSplitterFactory
{
    private const int AutoSplittersRequestTimeoutMilliseconds = 15000;

    private static readonly TimeSpan AutoSplittersRefreshInterval = TimeSpan.FromHours(2);

    private readonly object autoSplittersLock = new();
    private readonly object refreshLock = new();
    private bool initialized;
    private IDictionary<string, AutoSplitter> autoSplitters;

    public static AutoSplitterFactory Instance { get; protected set; }

    public bool NeedsRefresh => ShouldRefreshAutoSplitters();

    public IDictionary<string, AutoSplitter> AutoSplitters
    {
        get
        {
            lock (autoSplittersLock)
            {
                return autoSplitters;
            }
        }
        set
        {
            lock (autoSplittersLock)
            {
                autoSplitters = value;
            }
        }
    }

    public const string AutoSplittersXmlUrl = "https://cdn.jsdelivr.net/gh/LiveSplit/LiveSplit.AutoSplitters@master/LiveSplit.AutoSplitters.xml";
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
        lock (autoSplittersLock)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
        }

        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        XmlDocument localDocument = LoadAutoSplitters(AutoSplittersXmlFile);
        if (localDocument != null)
        {
            SetAutoSplitters(localDocument);
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

        XmlElement runtimeElement = element["AutoSplittingRuntime"];
        AutoSplittingRuntimeInfo runtimeInfo = null;
        if (runtimeElement != null)
        {
            runtimeInfo = new AutoSplittingRuntimeInfo
            {
                URL = GitHubUrlConverter.ToJsDelivr(runtimeElement["URL"]?.InnerText),
                Description = runtimeElement["Description"]?.InnerText,
                Website = runtimeElement["Website"]?.InnerText
            };
        }

        return new AutoSplitter()
        {
            Description = element["Description"].InnerText,
            URLs = [.. element["URLs"].ChildNodes.OfType<XmlElement>().Select(x => GitHubUrlConverter.ToJsDelivr(x.InnerText))],
            Type = autoSplitterType.Value,
            Games = element["Games"].ChildNodes.OfType<XmlElement>().Select(x => (x.InnerText ?? "").ToLower()).ToList(),
            ShowInLayoutEditor = element["ShowInLayoutEditor"] != null,
            Website = element["Website"]?.InnerText,
            AutoSplittingRuntime = runtimeInfo
        };
    }

    public AutoSplitter Create(string game)
    {
        Init();

        IDictionary<string, AutoSplitter> splitters = AutoSplitters;
        if (splitters != null && !string.IsNullOrEmpty(game))
        {
            game = game.ToLower();

            if (splitters.ContainsKey(game))
            {
                return splitters[game];
            }
        }

        return null;
    }

    public Task RefreshIfStaleAsync(IProgress<AutoSplitterUpdateProgress> progress)
    {
        return Task.Run(() =>
        {
            lock (refreshLock)
            {
                if (!ShouldRefreshAutoSplitters())
                {
                    progress?.Report(new AutoSplitterUpdateProgress(100, UiLocalizer.Translate("Auto Splitter list is up to date.")));
                    return;
                }

                try
                {
                    progress?.Report(new AutoSplitterUpdateProgress(0, UiLocalizer.Translate("Updating Auto Splitter list...")));
                    XmlDocument document = DownloadAutoSplitters(progress);
                    SetAutoSplitters(document);
                    progress?.Report(new AutoSplitterUpdateProgress(100, UiLocalizer.Translate("Auto Splitter list updated.")));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    progress?.Report(new AutoSplitterUpdateProgress(100, UiLocalizer.Translate("Failed to update Auto Splitter list.")));
                }
            }
        });
    }

    protected XmlDocument DownloadAutoSplitters(IProgress<AutoSplitterUpdateProgress> progress = null)
    {
        var request = (HttpWebRequest)WebRequest.Create(AutoSplittersXmlUrl);
        request.Timeout = AutoSplittersRequestTimeoutMilliseconds;
        request.ReadWriteTimeout = AutoSplittersRequestTimeoutMilliseconds;
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using WebResponse response = request.GetResponse();
        using Stream stream = response.GetResponseStream();
        using var memoryStream = new MemoryStream();
        long totalLength = response.ContentLength;
        long received = 0;
        byte[] buffer = new byte[64 * 1024];
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            memoryStream.Write(buffer, 0, read);
            received += read;
            double percentage = totalLength > 0 ? received * 100d / totalLength : 0;
            progress?.Report(new AutoSplitterUpdateProgress(
                percentage,
                string.Format(UiLocalizer.Translate("Downloading Auto Splitter list... {0}%"), (int)Math.Round(percentage))));
        }

        memoryStream.Position = 0;
        XmlDocument autoSplitters = LoadAutoSplitters(memoryStream);
        SaveAutoSplitters(autoSplitters);
        return autoSplitters;
    }
    private void SetAutoSplitters(XmlDocument document)
    {
        if (document?.DocumentElement == null)
        {
            return;
        }

        try
        {
            IDictionary<string, AutoSplitter> splitters = document["AutoSplitters"].ChildNodes.OfType<XmlElement>()
                .Where(element => element != null)
                .Select(CreateFromXmlElement)
                .SelectMany(x => x.Games.Select(y => new KeyValuePair<string, AutoSplitter>(y, x)))
                .ToDictionary(x => x.Key, x => x.Value);

            AutoSplitters = splitters;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private static bool ShouldRefreshAutoSplitters()
    {
        try
        {
            return !File.Exists(AutoSplittersXmlFile)
                || File.GetLastWriteTimeUtc(AutoSplittersXmlFile) < DateTime.UtcNow - AutoSplittersRefreshInterval;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    private static XmlDocument LoadAutoSplitters(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            return LoadAutoSplitters(stream);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    private static XmlDocument LoadAutoSplitters(Stream stream)
    {
        var autoSplitters = new XmlDocument() { XmlResolver = null };
        using var reader = XmlReader.Create(stream, new XmlReaderSettings()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        });
        autoSplitters.Load(reader);
        return autoSplitters;
    }

    private static void SaveAutoSplitters(XmlDocument document)
    {
        string fullPath = Path.GetFullPath(AutoSplittersXmlFile);
        string directory = Path.GetDirectoryName(fullPath);
        string temporaryPath = Path.Combine(directory, Path.GetRandomFileName());

        try
        {
            document.Save(temporaryPath);
            File.Copy(temporaryPath, fullPath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}