using LiveSplit.Localization;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Model;

public enum AutoSplitterType
{
    Component,
    Script,
    AutoSplittingRuntimeScript
}
public class AutoSplittingRuntimeInfo
{
    public string URL { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
}
public class AutoSplitter : ICloneable
{
    private const int DownloadTimeoutMilliseconds = 15000;
    private const int MetadataTimeoutMilliseconds = 2000;
    private static readonly TimeSpan LastModifiedTolerance = TimeSpan.FromMinutes(1);

    private readonly object downloadLock = new();
    private bool isActivating;

    public string Description { get; set; }
    public IEnumerable<string> Games { get; set; }
    public bool IsActivated => Component != null;
    public List<string> URLs { get; set; }
    public string LocalPath => Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, FileName));
    public string FileName => GetFileName(URLs[0]);
    public AutoSplitterType Type { get; set; }
    public bool ShowInLayoutEditor { get; set; }
    public IComponent Component { get; set; }
    public IComponentFactory Factory { get; set; }
    public bool IsDownloaded => File.Exists(LocalPath);
    public string Website { get; set; }
    public AutoSplittingRuntimeInfo AutoSplittingRuntime { get; set; }

    public void Activate(LiveSplitState state)
    {
        if (IsActivated || isActivating)
        {
            return;
        }

        isActivating = true;
        try
        {
            DownloadFiles(forceUpdate: false, progress: null);
            ActivateDownloadedFiles(state);
        }
        catch (Exception ex)
        {
            ShowActivationError(state, ex);
        }
        finally
        {
            isActivating = false;
        }
    }

    public void ActivateFromLocal(LiveSplitState state)
    {
        if (IsActivated || isActivating || !File.Exists(LocalPath))
        {
            return;
        }

        isActivating = true;
        try
        {
            ActivateDownloadedFiles(state);
        }
        catch (Exception ex)
        {
            ShowActivationError(state, ex);
        }
        finally
        {
            isActivating = false;
        }
    }
    public async Task ActivateAsync(LiveSplitState state, IProgress<AutoSplitterUpdateProgress> progress)
    {
        if (IsActivated || isActivating)
        {
            return;
        }

        isActivating = true;
        try
        {
            progress?.Report(new AutoSplitterUpdateProgress(0, UiLocalizer.Translate("Checking auto splitter files...")));
            await Task.Run(() => DownloadFiles(forceUpdate: false, progress)).ConfigureAwait(true);
            progress?.Report(new AutoSplitterUpdateProgress(95, UiLocalizer.Translate("Activating auto splitter...")));
            ActivateDownloadedFiles(state);
            progress?.Report(new AutoSplitterUpdateProgress(100, UiLocalizer.Translate("Auto splitter ready.")));
        }
        catch (Exception ex)
        {
            ShowActivationError(state, ex);
        }
        finally
        {
            isActivating = false;
        }
    }

    public Task<bool> ForceUpdateAsync(IProgress<AutoSplitterUpdateProgress> progress)
    {
        return Task.Run(() =>
        {
            bool succeeded = DownloadFiles(forceUpdate: true, progress);
            progress?.Report(new AutoSplitterUpdateProgress(
                100,
                succeeded ? UiLocalizer.Translate("Update complete.") : UiLocalizer.Translate("Update finished with errors.")));
            return succeeded;
        });
    }

    private void ActivateDownloadedFiles(LiveSplitState state)
    {
        RegisterDownloadedFactories();

        switch (Type)
        {
            case AutoSplitterType.Component:
                Factory = ComponentManager.ComponentFactories[FileName];
                Component = Factory.Create(state);
                break;
            case AutoSplitterType.Script:
                Factory = ComponentManager.ComponentFactories["LiveSplit.ScriptableAutoSplit.dll"];
                Component = ((dynamic)Factory).Create(state, LocalPath);
                break;
            case AutoSplitterType.AutoSplittingRuntimeScript:
                Factory = ComponentManager.ComponentFactories["LiveSplit.AutoSplittingRuntime.dll"];
                Component = ((dynamic)Factory).Create(state, LocalPath);
                break;
        }
    }

    private bool DownloadFiles(bool forceUpdate, IProgress<AutoSplitterUpdateProgress> progress)
    {
        lock (downloadLock)
        {
            return DownloadFilesCore(forceUpdate, progress);
        }
    }

    private bool DownloadFilesCore(bool forceUpdate, IProgress<AutoSplitterUpdateProgress> progress)
    {
        DownloadCandidate[] candidates = URLs.Select(url =>
        {
            string convertedUrl = GitHubUrlConverter.ToJsDelivr(url);
            string fileName = GetFileName(convertedUrl);
            string localPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, fileName));
            return new DownloadCandidate(convertedUrl, fileName, localPath);
        }).ToArray();

        var filesToDownload = new List<DownloadItem>();
        for (int index = 0; index < candidates.Length; index++)
        {
            DownloadCandidate candidate = candidates[index];
            double checkPercentage = 5 + index * 20d / candidates.Length;
            progress?.Report(new AutoSplitterUpdateProgress(
                checkPercentage,
                string.Format(UiLocalizer.Translate("Checking remote file {0}/{1}: {2}"), index + 1, candidates.Length, candidate.FileName)));

            candidate.RemoteInfo = GetRemoteFileInfo(candidate.ConvertedUrl);
            if (forceUpdate || ShouldUpdate(candidate.LocalPath, candidate.RemoteInfo))
            {
                filesToDownload.Add(new DownloadItem(
                    candidate.ConvertedUrl,
                    AddCacheBuster(candidate.ConvertedUrl),
                    candidate.FileName,
                    candidate.LocalPath,
                    candidate.RemoteInfo?.Length ?? 0));
            }
        }

        if (filesToDownload.Count == 0)
        {
            progress?.Report(new AutoSplitterUpdateProgress(90, UiLocalizer.Translate("Auto splitter files are up to date.")));
            return true;
        }

        bool allSucceeded = true;
        for (int index = 0; index < filesToDownload.Count; index++)
        {
            DownloadItem item = filesToDownload[index];
            string tempFileName = item.FileName + "-temp";
            string tempLocalPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, tempFileName));

            try
            {
                progress?.Report(new AutoSplitterUpdateProgress(
                    25 + index * 65d / filesToDownload.Count,
                    string.Format(UiLocalizer.Translate("Downloading {0}/{1}: {2}"), index + 1, filesToDownload.Count, item.FileName)));
                DownloadFileWithProgress(
                    item.DownloadUrl,
                    tempLocalPath,
                    item.Length,
                    (received, total) =>
                    {
                        double fileProgress = total > 0 ? received / (double)total : 0;
                        double percentage = 25 + (index + fileProgress) * 65d / filesToDownload.Count;
                        progress?.Report(new AutoSplitterUpdateProgress(
                            percentage,
                            string.Format(UiLocalizer.Translate("Downloading {0}/{1}: {2}"), index + 1, filesToDownload.Count, item.FileName)));
                    });

                File.Copy(tempLocalPath, item.LocalPath, true);
            }
            catch (WebException)
            {
                allSucceeded = false;
                Log.Error("Error downloading file from " + item.ConvertedUrl);
            }
            catch (Exception ex)
            {
                allSucceeded = false;
                Log.Error(ex);
            }
            finally
            {
                try
                {
                    File.Delete(tempLocalPath);
                }
                catch (Exception)
                {
                    Log.Error($"Failed to delete temp file: {tempLocalPath}");
                }
            }
        }

        progress?.Report(new AutoSplitterUpdateProgress(90, allSucceeded ? UiLocalizer.Translate("Download complete.") : UiLocalizer.Translate("Download finished with errors.")));
        return allSucceeded;
    }
    private static bool ShouldUpdate(string localPath, RemoteFileInfo remoteInfo)
    {
        if (!File.Exists(localPath))
        {
            return true;
        }

        if (remoteInfo == null)
        {
            return false;
        }

        var localFile = new FileInfo(localPath);
        if (remoteInfo.Length > 0 && localFile.Length != remoteInfo.Length)
        {
            return true;
        }

        return remoteInfo.LastModifiedUtc.HasValue
            && localFile.LastWriteTimeUtc + LastModifiedTolerance < remoteInfo.LastModifiedUtc.Value;
    }

    private RemoteFileInfo GetRemoteFileInfo(string url)
    {
        try
        {
            string metadataUrl = AddCacheBuster(GetMetadataUrl(url));
            return GetRemoteFileInfoFromRange(metadataUrl, null);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }
    private static RemoteFileInfo GetRemoteFileInfoFromRange(string url, DateTime? lastModifiedUtc)
    {
        try
        {
            var request = CreateRequest(url, MetadataTimeoutMilliseconds);
            request.AddRange(0, 0);

            using var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.PartialContent
                && TryParseContentRange(response.Headers["Content-Range"], out long length))
            {
                return new RemoteFileInfo(length, GetLastModified(response) ?? lastModifiedUtc);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return lastModifiedUtc.HasValue ? new RemoteFileInfo(0, lastModifiedUtc) : null;
    }

    private static DateTime? GetLastModified(HttpWebResponse response)
    {
        string value = response.Headers[HttpResponseHeader.LastModified];
        return DateTimeOffset.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out DateTimeOffset lastModified)
            ? lastModified.UtcDateTime
            : null;
    }

    private static bool TryParseContentRange(string value, out long length)
    {
        length = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        int separator = value.LastIndexOf('/');
        return separator >= 0
            && separator + 1 < value.Length
            && long.TryParse(value[(separator + 1)..], out length);
    }

    private static string GetMetadataUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
            || !uri.Host.Equals("cdn.jsdelivr.net", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        var builder = new UriBuilder(uri)
        {
            Host = "fastly.jsdelivr.net"
        };
        return builder.Uri.AbsoluteUri;
    }

    private static HttpWebRequest CreateRequest(string url, int timeoutMilliseconds)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Timeout = timeoutMilliseconds;
        request.ReadWriteTimeout = timeoutMilliseconds;
        request.UserAgent = "LiveSplit";
        return request;
    }
    private static void DownloadFileWithProgress(
        string url,
        string destinationPath,
        long expectedLength,
        Action<long, long> reportProgress)
    {
        var request = CreateRequest(url, DownloadTimeoutMilliseconds);
        using var response = (HttpWebResponse)request.GetResponse();
        long totalLength = expectedLength > 0 ? expectedLength : response.ContentLength;
        long received = 0;
        byte[] buffer = new byte[64 * 1024];

        using (Stream input = response.GetResponseStream())
        using (FileStream output = File.Create(destinationPath))
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
                received += read;
                reportProgress?.Invoke(received, totalLength);
            }
        }

        if (totalLength > 0 && received != totalLength)
        {
            throw new InvalidDataException("The downloaded file was incomplete.");
        }
    }

    private sealed class DownloadCandidate
    {
        public string ConvertedUrl { get; }
        public string FileName { get; }
        public string LocalPath { get; }
        public RemoteFileInfo RemoteInfo { get; set; }

        public DownloadCandidate(string convertedUrl, string fileName, string localPath)
        {
            ConvertedUrl = convertedUrl;
            FileName = fileName;
            LocalPath = localPath;
        }
    }
    private sealed class DownloadItem
    {
        public string ConvertedUrl { get; }
        public string DownloadUrl { get; }
        public string FileName { get; }
        public string LocalPath { get; }
        public long Length { get; }

        public DownloadItem(string convertedUrl, string downloadUrl, string fileName, string localPath, long length)
        {
            ConvertedUrl = convertedUrl;
            DownloadUrl = downloadUrl;
            FileName = fileName;
            LocalPath = localPath;
            Length = length;
        }
    }

    private sealed class RemoteFileInfo
    {
        public long Length { get; }
        public DateTime? LastModifiedUtc { get; }

        public RemoteFileInfo(long length, DateTime? lastModifiedUtc)
        {
            Length = length;
            LastModifiedUtc = lastModifiedUtc;
        }
    }
    private static string AddCacheBuster(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
            || !uri.Host.EndsWith("jsdelivr.net", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        var builder = new UriBuilder(uri);
        string query = builder.Query.TrimStart('?');
        builder.Query = string.IsNullOrEmpty(query)
            ? "livesplit=" + Guid.NewGuid().ToString("N")
            : query + "&livesplit=" + Guid.NewGuid().ToString("N");
        return builder.Uri.AbsoluteUri;
    }

    private static string GetFileName(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        {
            string path = uri.AbsolutePath;
            return path[(path.LastIndexOf('/') + 1)..];
        }

        return url[(url.LastIndexOf('/') + 1)..];
    }
    private void RegisterDownloadedFactories()
    {
        foreach (string url in URLs.Skip(1))
        {
            string convertedUrl = GitHubUrlConverter.ToJsDelivr(url);
            string fileName = GetFileName(convertedUrl);
            string localPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, fileName));
            if (!localPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || !File.Exists(localPath))
            {
                continue;
            }

            IComponentFactory factory = ComponentManager.LoadFactory<IComponentFactory>(localPath);
            if (factory != null && !ComponentManager.ComponentFactories.ContainsKey(fileName))
            {
                ComponentManager.ComponentFactories.Add(fileName, factory);
            }
        }

        if (Type == AutoSplitterType.Component && !ComponentManager.ComponentFactories.ContainsKey(FileName))
        {
            IComponentFactory factory = ComponentManager.LoadFactory<IComponentFactory>(LocalPath);
            if (factory != null)
            {
                ComponentManager.ComponentFactories.Add(Path.GetFileName(LocalPath), factory);
            }
        }
    }

    private static void ShowActivationError(LiveSplitState state, Exception ex)
    {
        Log.Error(ex);
        MessageBox.Show(state.Form, string.Format(UiLocalizer.Translate("The Auto Splitter could not be activated. ({0})"), ex.Message), UiLocalizer.Translate("Activation Failed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void Deactivate()
    {
        if (IsActivated)
        {
            Component.Dispose();
            Component = null;
        }
    }

    public AutoSplitter Clone()
    {
        return new AutoSplitter()
        {
            Description = Description,
            Games = [.. Games],
            URLs = [.. URLs],
            Type = Type,
            ShowInLayoutEditor = ShowInLayoutEditor,
            Component = Component,
            Factory = Factory,
            Website = Website,
            AutoSplittingRuntime = AutoSplittingRuntime == null ? null : new AutoSplittingRuntimeInfo
            {
                URL = AutoSplittingRuntime.URL,
                Description = AutoSplittingRuntime.Description,
                Website = AutoSplittingRuntime.Website
            }
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}