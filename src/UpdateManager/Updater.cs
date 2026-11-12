using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace UpdateManager;

public class UpdatePercentageRefreshedEventArgs : EventArgs
{
    public double Percentage { get; protected set; }

    public UpdatePercentageRefreshedEventArgs(double percentage)
    {
        Percentage = percentage;
    }
}

public delegate void UpdatePercentageRefreshedEventHandler(object sender, UpdatePercentageRefreshedEventArgs e);

public static class Updater
{
    private const int UpdateRequestTimeoutMilliseconds = 5000;
    private const int MaxUpdateCheckDurationMilliseconds = 8000;
    private const int MaxConcurrentUpdateChecks = 4;
    private const string UpdateManagerVersion = "2.0.5";

    private static readonly HttpClient UpdateHttpClient = CreateUpdateHttpClient();

    private static readonly ConcurrentDictionary<IUpdateable, UpdaterInternal> Updaters = new();

    #region Updater Instance

    internal class UpdaterInternal
    {
        public string XMLURL { get; set; }
        public string UpdateURL { get; set; }
        public Version Version { get; set; }

        private readonly object updatesLock = new();
        private IEnumerable<Update> _Updates { get; set; }
        public IEnumerable<Update> Updates
        {
            get
            {
                lock (updatesLock)
                {
                    _Updates ??= DownloadUpdates();
                    return _Updates;
                }
            }
        }

        private IEnumerable<Update> DownloadUpdates()
        {
            List<Update> updateList = [];
            if (!Uri.TryCreate(XMLURL, UriKind.Absolute, out Uri uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return updateList;
            }

            try
            {
                string xml = UpdateHttpClient.GetStringAsync(uri).GetAwaiter().GetResult();
                var doc = new XmlDocument() { XmlResolver = null };
                using var stringReader = new StringReader(xml);
                using var reader = XmlReader.Create(stringReader, new XmlReaderSettings()
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                });
                doc.Load(reader);

                if (doc.DocumentElement == null)
                {
                    return updateList;
                }

                foreach (XmlNode updateNode in doc.DocumentElement.ChildNodes)
                {
                    if (updateNode.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    try
                    {
                        updateList.Add(Update.Parse(updateNode));
                    }
                    catch { }
                }
            }
            catch { }

            return updateList;
        }

        public event UpdatePercentageRefreshedEventHandler UpdatePercentageRefreshed;

        internal UpdaterInternal(string xmlUrl, string updateUrl, Version version)
        {
            XMLURL = ToJsDelivrUrl(xmlUrl) ?? xmlUrl;
            UpdateURL = ToJsDelivrUrl(updateUrl) ?? updateUrl;
            Version = version;
        }

        public bool CheckForUpdate()
        {
            foreach (Update update in Updates)
            {
                if (update.Version > Version)
                {
                    return true;
                }
            }

            return false;
        }

        public Version GetNewVersion()
        {
            return Updates.Max(x => x.Version);
        }

        public IEnumerable<string> GetChangeLog()
        {
            return Updates.Where(x => x.Version > Version).SelectMany(x => x.ChangeLog);
        }

        public void PerformUpdate()
        {
            static string convertChangeUrlPartToPath(string xmlChangePath)
            {
                return xmlChangePath.Replace('/', Path.DirectorySeparatorChar);
            }

            IList<Update> updates = [.. Updates.Where(x => x.Version > Version)];
            var addedFiles = new Dictionary<string, string>();
            var changedFiles = new Dictionary<string, string>();
            var removedFiles = new Dictionary<string, string>();

            foreach (FileChange change in updates.SelectMany(x => x.FileChanges))
            {
                string path = change.Path;
                string localPath = change.LocalPath ?? change.Path;

                switch (change.Status)
                {
                    case ChangeStatus.Added:
                        removedFiles.Remove(path);
                        addedFiles[path] = localPath;
                        break;
                    case ChangeStatus.Changed:
                        changedFiles[path] = localPath;
                        break;
                    case ChangeStatus.Removed:
                        addedFiles.Remove(path);
                        changedFiles.Remove(path);
                        removedFiles[path] = localPath;
                        break;
                }
            }

            int fileChangesCount = addedFiles.Count + changedFiles.Count + removedFiles.Count;
            double i = 0;

            foreach (KeyValuePair<string, string> xmlChangePaths in addedFiles.Concat(changedFiles))
            {
                string path = xmlChangePaths.Key;
                string localPath = xmlChangePaths.Value;
                DownloadFile(UpdateURL + path, convertChangeUrlPartToPath(localPath));
                UpdatePercentageRefreshed?.Invoke(this, new UpdatePercentageRefreshedEventArgs(++i / fileChangesCount));
            }

            foreach (KeyValuePair<string, string> xmlChangePaths in removedFiles)
            {
                string localPath = xmlChangePaths.Value;
                File.Delete(convertChangeUrlPartToPath(localPath));
                UpdatePercentageRefreshed?.Invoke(this, new UpdatePercentageRefreshedEventArgs(++i / fileChangesCount));
            }
        }
    }

    #endregion

    #region Static Methods

    private static void DownloadFile(string url, string path)
    {
        string dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), path));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        new WebClient().DownloadFile(url, path);
    }

    public static void UpdateAll(IEnumerable<IUpdateable> updateables, string updateManagerDownloadURL, string updateManagerConfigDownloadUrl)
    {
        DownloadFile(updateManagerDownloadURL, "UpdateManager.exe");
        DownloadFile(updateManagerConfigDownloadUrl, "UpdateManager.exe.config");
        string arguments = string.Concat(updateables.Where(x => x.CheckForUpdate()).Select(x =>
        {
            UpdaterInternal updater = GetUpdater(x);
            return $"\"{updater.XMLURL}\" \"{updater.UpdateURL}\" {x.Version} ";
        })) + $"\"{Process.GetCurrentProcess().ProcessName}.exe\"";
        Process.Start("UpdateManager.exe", arguments);
    }

    internal static void UpdateAllInternally(IEnumerable<IUpdateable> updateables)
    {
        foreach (IUpdateable updateable in updateables.Where(x => x.CheckForUpdate()))
        {
            GetUpdater(updateable).PerformUpdate();
        }
    }

    public static IEnumerable<string> GetChangeLogFromAll(IEnumerable<IUpdateable> updateables)
    {
        return updateables.Where(x => x.CheckForUpdate()).SelectMany(x => x.GetChangeLog());
    }

    internal static UpdaterInternal GetUpdater(IUpdateable updateable)
    {
        return Updaters.GetOrAdd(
            updateable,
            x => new UpdaterInternal(x.XMLURL, x.UpdateURL, x.Version));
    }

    public static bool CheckForAnyUpdate(IEnumerable<IUpdateable> updateables)
    {
        bool updateAvailable = false;
        using var cancellation = new CancellationTokenSource(MaxUpdateCheckDurationMilliseconds);

        try
        {
            Parallel.ForEach(
                updateables,
                new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxConcurrentUpdateChecks,
                    CancellationToken = cancellation.Token
                },
                updateable =>
                {
                    if (updateable.CheckForUpdate())
                    {
                        Volatile.Write(ref updateAvailable, true);
                    }
                });
        }
        catch (OperationCanceledException) { }

        return Volatile.Read(ref updateAvailable);
    }

    private static string ToJsDelivrUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)
            || !Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return null;
        }

        string[] segments = uri.AbsolutePath.Trim('/').Split('/');
        if (uri.Host.Equals("raw.githubusercontent.com", StringComparison.OrdinalIgnoreCase))
        {
            return segments.Length >= 3 ? CreateJsDelivrUrl(uri, segments, 0, 1, 2) : null;
        }

        if ((uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)
                || uri.Host.Equals("www.github.com", StringComparison.OrdinalIgnoreCase))
            && segments.Length >= 4
            && (segments[2].Equals("raw", StringComparison.OrdinalIgnoreCase)
                || segments[2].Equals("blob", StringComparison.OrdinalIgnoreCase)))
        {
            return CreateJsDelivrUrl(uri, segments, 0, 1, 3);
        }

        return null;
    }

    private static string CreateJsDelivrUrl(Uri uri, string[] segments, int ownerIndex, int repositoryIndex, int referenceIndex)
    {
        string reference;
        int contentIndex;
        if (segments[referenceIndex].Equals("refs", StringComparison.OrdinalIgnoreCase)
            && referenceIndex + 2 < segments.Length
            && (segments[referenceIndex + 1].Equals("heads", StringComparison.OrdinalIgnoreCase)
                || segments[referenceIndex + 1].Equals("tags", StringComparison.OrdinalIgnoreCase)))
        {
            reference = segments[referenceIndex + 2];
            contentIndex = referenceIndex + 3;
        }
        else
        {
            reference = segments[referenceIndex];
            contentIndex = referenceIndex + 1;
        }

        string contentPath = contentIndex < segments.Length
            ? string.Join("/", segments, contentIndex, segments.Length - contentIndex)
            : string.Empty;
        return $"https://cdn.jsdelivr.net/gh/{segments[ownerIndex]}/{segments[repositoryIndex]}@{reference}/{contentPath}{uri.Query}";
    }
    private static HttpClient CreateUpdateHttpClient()
    {
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            MaxConnectionsPerServer = MaxConcurrentUpdateChecks
        };
        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMilliseconds(UpdateRequestTimeoutMilliseconds)
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd($"LiveSplit-UpdateManager/{UpdateManagerVersion}");
        return client;
    }

    #endregion

    #region Extensions

    public static Version GetNewVersion(this IUpdateable updateable)
    {
        return GetUpdater(updateable).GetNewVersion();
    }

    public static bool CheckForUpdate(this IUpdateable updateable)
    {
        return GetUpdater(updateable).CheckForUpdate();
    }

    public static IEnumerable<string> GetChangeLog(this IUpdateable updateable)
    {
        return GetUpdater(updateable).GetChangeLog();
    }

    #endregion
}
