using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
    private Task _preparation;
    private int _activationVersion;
    public string Description { get; set; }
    public IEnumerable<string> Games { get; set; }
    public bool IsActivated => Component != null;
    public List<string> URLs { get; set; }
    public string LocalPath => Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, FileName));
    public string FileName => URLs[0][(URLs[0].LastIndexOf('/') + 1)..];
    public AutoSplitterType Type { get; set; }
    public bool ShowInLayoutEditor { get; set; }
    public IComponent Component { get; set; }
    public IComponentFactory Factory { get; set; }
    public bool IsDownloaded => File.Exists(LocalPath);
    public string Website { get; set; }
    public AutoSplittingRuntimeInfo AutoSplittingRuntime { get; set; }

    public void Activate(LiveSplitState state) => Activate(state, true);

    public async Task ActivateAsync(LiveSplitState state, bool allowWhileRunning = false)
    {
        IRun run = state.Run;
        string game = run.GameName;
        int version = _activationVersion;
        if (IsActivated || (!allowWhileRunning && state.CurrentPhase != TimerPhase.NotRunning)
            || !state.Settings.ActiveAutoSplitters.Contains(game))
        {
            return;
        }

        if (!IsDownloaded || Type == AutoSplitterType.Script || Type == AutoSplitterType.AutoSplittingRuntimeScript)
        {
            Task preparation = _preparation ??= Task.Run(DownloadFilesAsync);
            await preparation;
            if (!IsDownloaded && _preparation == preparation)
            {
                _preparation = null;
            }
        }

        // Downloads may finish after the user switches games, starts a run, or closes the window.
        if (state.Form.IsDisposed || state.Form.Disposing || state.Run != run || run.GameName != game
            || run.AutoSplitter != this || IsActivated || version != _activationVersion
            || (!allowWhileRunning && state.CurrentPhase != TimerPhase.NotRunning)
            || !state.Settings.ActiveAutoSplitters.Contains(game) || !IsDownloaded)
        {
            return;
        }

        Activate(state, false);
        if (IsActivated && run.AutoSplitterSettings != null
            && run.AutoSplitterSettings.GetAttribute("gameName") == game)
        {
            Component.SetSettings(run.AutoSplitterSettings);
        }
    }

    private void Activate(LiveSplitState state, bool downloadFiles)
    {
        if (!IsActivated)
        {
            try
            {
                if (downloadFiles && (!IsDownloaded || Type == AutoSplitterType.Script || Type == AutoSplitterType.AutoSplittingRuntimeScript))
                {
                    DownloadFilesAsync().GetAwaiter().GetResult();
                }

                RegisterFactories();
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
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(state.Form, "The Auto Splitter could not be activated. (" + ex.Message + ")", "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async Task DownloadFilesAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        foreach (string url in URLs)
        {
            string fileName = url[(url.LastIndexOf('/') + 1)..];
            string localPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, fileName));
            string tempLocalPath = localPath + "." + Guid.NewGuid() + ".tmp";

            try
            {
                // Download to temp file so the original file is kept if it fails downloading
                byte[] data = await client.GetByteArrayAsync(url).ConfigureAwait(false);
                File.WriteAllBytes(tempLocalPath, data);
                File.Copy(tempLocalPath, localPath, true);
            }
            catch (WebException)
            {
                Log.Error("Error downloading file from " + url);
            }
            catch (Exception ex)
            {
                // Catch errors of File.Copy() if necessary
                Log.Error(ex);
            }
            finally
            {
                try
                {
                    // This is not required to run the AutoSplitter, but should still try to clean up
                    File.Delete(tempLocalPath);
                }
                catch (Exception)
                {
                    Log.Error($"Failed to delete temp file: {tempLocalPath}");
                }
            }
        }

    }

    private void RegisterFactories()
    {
        foreach (string url in URLs)
        {
            string fileName = url[(url.LastIndexOf('/') + 1)..];
            if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                && !ComponentManager.ComponentFactories.ContainsKey(fileName))
            {
                string localPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, fileName));
                IComponentFactory factory = ComponentManager.LoadFactory<IComponentFactory>(localPath);
                if (factory != null)
                {
                    ComponentManager.ComponentFactories.Add(fileName, factory);
                }
            }
        }
    }

    public void Deactivate()
    {
        _activationVersion++;
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
