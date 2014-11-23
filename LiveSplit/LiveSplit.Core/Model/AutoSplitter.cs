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
    public class AutoSplitter
    {
        public String Description { get; set; }
        public IEnumerable<String> Games { get; set; }
        public bool IsActivated { get { return Component != null; } }
        public List<String> URLs { get; set; }
        public String LocalPath { get { return Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, FileName)); } }
        public String FileName { get { return URLs.First().Substring(URLs.First().LastIndexOf('/') + 1); } }
        public AutoSplitterType Type { get; set; }
        public bool ShowInLayoutEditor { get; set; }
        public IComponent Component { get; set; }
        public IComponentFactory Factory { get; set; }
        public bool IsDownloaded { get { return File.Exists(LocalPath); } }

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
                    MessageBox.Show(state.Form, "The Auto Splitter could not be activated.", "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DownloadFiles()
        {
            var client = new WebClient();

            foreach (var url in URLs)
            {
                var fileName = url.Substring(url.LastIndexOf('/') + 1);
                var localPath = Path.GetFullPath(Path.Combine(ComponentManager.BasePath ?? "", ComponentManager.PATH_COMPONENTS, fileName));

                try
                {
                    client.DownloadFile(new Uri(url), localPath);
                }
                catch (WebException)
                {
                    Log.Error("Error downloading file from " + url);
                }
            }

            if (Type == AutoSplitterType.Component)
            {
                var factory = ComponentManager.LoadFactory(LocalPath);
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
    }
}
