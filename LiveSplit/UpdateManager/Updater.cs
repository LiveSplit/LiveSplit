using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace UpdateManager
{
    public class UpdatePercentageRefreshedEventArgs : EventArgs
    {
        public double Percentage { get; protected set; }

        public UpdatePercentageRefreshedEventArgs(double percentage)
        {
            Percentage = percentage;
        }
    }

    public delegate void UpdatePercentageRefreshedEventHandler (object sender, UpdatePercentageRefreshedEventArgs e);

    public static class Updater
    {
        private static Dictionary<IUpdateable, UpdaterInternal> _Updaters { get; set; }
        private static Dictionary<IUpdateable, UpdaterInternal> Updaters
        {
            get
            {
                if (_Updaters == null)
                    _Updaters = new Dictionary<IUpdateable, UpdaterInternal>();
                return _Updaters;
            }
        }

        #region Updater Instance

        internal class UpdaterInternal
        {
            public String XMLURL { get; set; }
            public String UpdateURL { get; set; }
            public Version Version { get; set; }

            private IEnumerable<Update> _Updates { get; set; }
            public IEnumerable<Update> Updates
            {
                get
                {
                    if (_Updates == null)
                    {
                        List<Update> updateList = new List<Update>();
                        try
                        {
                            using (XmlReader reader = XmlReader.Create(XMLURL))
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.Load(reader);
                                foreach (XmlNode updateNode in doc.DocumentElement.ChildNodes)
                                {
                                    Update update = Update.Parse(updateNode);
                                    updateList.Add(update);
                                }
                            }
                        }
                        catch { }
                        _Updates = updateList;
                    }

                    return _Updates;
                }
            }

            public event UpdatePercentageRefreshedEventHandler UpdatePercentageRefreshed;

            internal UpdaterInternal(String xmlUrl, String updateUrl, Version version)
            {
                XMLURL = xmlUrl;
                UpdateURL = updateUrl;
                Version = version;
            }

            public bool CheckForUpdate()
            {
                foreach (Update update in Updates)
                {
                    if (update.Version > Version)
                        return true;
                }
                return false;
            }

            public Version GetNewVersion()
            {
                return Updates.Max(x => x.Version);
            }

            public IEnumerable<String> GetChangeLog()
            {
                return Updates.Where(x => x.Version > Version).SelectMany(x => x.ChangeLog);
            }

            public void PerformUpdate()
            {
                IList<Update> updates = Updates.Where(x => x.Version > Version).ToList();
                List<String> addedFiles = new List<string>();
                List<String> changedFiles = new List<string>();
                List<String> removedFiles = new List<string>();

                foreach (FileChange change in updates.SelectMany(x => x.FileChanges))
                {
                    bool inAddedFiles = addedFiles.Contains(change.Path);
                    bool inChangedFiles = changedFiles.Contains(change.Path);
                    bool inRemovedFiles = removedFiles.Contains(change.Path);

                    String path = change.Path;

                    switch (change.Status)
                    {
                        case ChangeStatus.Added:
                            if (!inAddedFiles)
                                addedFiles.Add(path);
                            if (inRemovedFiles)
                                removedFiles.Remove(path);
                            break;
                        case ChangeStatus.Changed:
                            if (!inChangedFiles)
                                changedFiles.Add(path);
                            break;
                        case ChangeStatus.Removed:
                            if (inAddedFiles)
                                addedFiles.Remove(path);
                            if (inChangedFiles)
                                changedFiles.Remove(path);
                            if (!inRemovedFiles)
                                removedFiles.Add(path);
                            break;
                    }
                }

                int fileChangesCount = addedFiles.Concat(changedFiles).Concat(removedFiles).Count();
                double i = 0;

                foreach (String path in addedFiles.Concat(changedFiles))
                {
                    DownloadFile(UpdateURL + path, path.Replace('/', '\\'));
                    UpdatePercentageRefreshed(this, new UpdatePercentageRefreshedEventArgs(++i / fileChangesCount));
                }

                foreach (String path in removedFiles)
                {
                    File.Delete(path);
                    UpdatePercentageRefreshed(this, new UpdatePercentageRefreshedEventArgs(++i / fileChangesCount));
                }
            }
        }

        #endregion

        #region Static Methods

        private static void DownloadFile(String url, String path)
        {
            WebRequest request = HttpWebRequest.Create(url);
            Thread t = Thread.CurrentThread;
            IAsyncResult result = request.BeginGetResponse(ar => t.Resume(), null);
            t.Suspend();
            WebResponse response = request.EndGetResponse(result);
            String directory = Path.GetDirectoryName(Directory.GetCurrentDirectory() + '\\' + path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (Stream webStream = response.GetResponseStream())
            {
                using (Stream fileStream = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = new BinaryReader(webStream).ReadBytes((int)response.ContentLength);
                    new BinaryWriter(fileStream).Write(bytes);
                }
            }
        }

        public static void UpdateAll(IEnumerable<IUpdateable> updateables, String updateManagerDownloadURL)
        {
            DownloadFile(updateManagerDownloadURL, "UpdateManager.exe");
            String arguments = updateables.Where(x => x.CheckForUpdate()).Aggregate("", (x, y) => x + "\"" + y.XMLURL + "\" \"" + y.UpdateURL + "\" " + y.Version + " ") + "\"" + Process.GetCurrentProcess().ProcessName + ".exe\"";
            Process.Start("UpdateManager.exe", arguments);
        }

        internal static void UpdateAllInternally(IEnumerable<IUpdateable> updateables)
        {
            foreach (IUpdateable updateable in updateables.Where(x => x.CheckForUpdate()))
                GetUpdater(updateable).PerformUpdate();
        }

        public static IEnumerable<String> GetChangeLogFromAll(IEnumerable<IUpdateable> updateables)
        {
            return updateables.Where(x => x.CheckForUpdate()).SelectMany(x => x.GetChangeLog());
        }

        internal static UpdaterInternal GetUpdater(IUpdateable updateable)
        {
            if (!Updaters.ContainsKey(updateable))
                Updaters.Add(updateable, new UpdaterInternal(updateable.XMLURL, updateable.UpdateURL, updateable.Version));

            return Updaters[updateable];
        }

        public static bool CheckForAnyUpdate(IEnumerable<IUpdateable> updateables)
        {
            return updateables.Any(x => x.CheckForUpdate());
        }

        #endregion

        #region Extensions

        public static void PerformUpdate(this IUpdateable updateable, String updateManagerDownloadURL)
        {
            UpdateAll(Enumerable.Repeat(updateable, 1), updateManagerDownloadURL);
        }

        public static Version GetNewVersion(this IUpdateable updateable)
        {
            return GetUpdater(updateable).GetNewVersion();
        }

        public static bool CheckForUpdate(this IUpdateable updateable)
        {
            return GetUpdater(updateable).CheckForUpdate();
        }

        public static IEnumerable<String> GetChangeLog(this IUpdateable updateable)
        {
            return GetUpdater(updateable).GetChangeLog();
        }

        #endregion
    }
}
