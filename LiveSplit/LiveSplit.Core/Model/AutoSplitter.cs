using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace LiveSplit.Model
{
    public enum AutoSplitterType
    {
        Component, Script
    }
    public class AutoSplitter : ICloneable
    {
        public string Description { get; set; }
        public IEnumerable<string> Games { get; set; }
        public bool IsActivated => Component != null;
        public List<string> URLs { get; set; }
        public string LocalPath => Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, FileName));
        public string FileName => URLs.First().Substring(URLs.First().LastIndexOf('/') + 1);
        public AutoSplitterType Type { get; set; }
        public bool ShowInLayoutEditor { get; set; }
        public IComponent Component { get; set; }
        public IComponentFactory Factory { get; set; }
        public bool IsDownloaded => File.Exists(LocalPath);
        public string Website { get; set; }

        public void Activate(LiveSplitState state)
        {
            if (!IsActivated)
            {
                try
                {
                    if (!IsDownloaded || Type == AutoSplitterType.Script)
                        DownloadFiles();
                    if (Type == AutoSplitterType.Component)
                    {
                        Factory = ComponentManager.ComponentFactories[FileName];
                        Component = Factory.Create(state);
                    }
                    else
                    {
                        Factory = ComponentManager.ComponentFactories["LiveSplit.ScriptableAutoSplit.dll"];
                        Component = ((dynamic)Factory).Create(state, LocalPath);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(state.Form, "The Auto Splitter could not be activated. (" + ex.Message + ")", "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DownloadFiles()
        {
            var client = new WebClient();

            foreach (var url in URLs)
            {
                var fileName = url.Substring(url.LastIndexOf('/') + 1);
                var tempFileName = fileName + "-temp";
                var localPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, fileName));
                var tempLocalPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, tempFileName));

                try
                {
                    // Download to temp file so the original file is kept if it fails downloading
                    client.DownloadFile(new Uri(url), tempLocalPath);
                    File.Copy(tempLocalPath, localPath, true);

                    if (url != URLs.First())
                    {
                        var factory = ComponentManager.LoadFactory<IComponentFactory>(localPath);
                        if (factory != null)
                            ComponentManager.ComponentFactories.Add(fileName, factory);
                    }
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

            if (Type == AutoSplitterType.Component)
            {
                var factory = ComponentManager.LoadFactory<IComponentFactory>(LocalPath);
                ComponentManager.ComponentFactories.Add(Path.GetFileName(LocalPath), factory);
            }
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
                Games = new List<string>(Games),
                URLs = new List<string>(URLs),
                Type = Type,
                ShowInLayoutEditor = ShowInLayoutEditor,
                Component = Component,
                Factory = Factory
            };
        }

        object ICloneable.Clone() => Clone();
    }
}
