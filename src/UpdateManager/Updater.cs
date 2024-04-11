using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            public string XMLURL { get; set; }
            public string UpdateURL { get; set; }
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

            internal UpdaterInternal(string xmlUrl, string updateUrl, Version version)
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

            public IEnumerable<string> GetChangeLog()
            {
                return Updates.Where(x => x.Version > Version).SelectMany(x => x.ChangeLog);
            }

            public void PerformUpdate()
            {
                string ConvertChangeUrlPartToPath(string xmlChangePath) => xmlChangePath.Replace('/', Path.DirectorySeparatorChar);

                IList<Update> updates = Updates.Where(x => x.Version > Version).ToList();
                var addedFiles = new Dictionary<string, string>();
                var changedFiles = new Dictionary<string, string>();
                var removedFiles = new Dictionary<string, string>();

                foreach (var change in updates.SelectMany(x => x.FileChanges))
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

                int fileChangesCount = addedFiles.Concat(changedFiles).Concat(removedFiles).Count();
                double i = 0;

                foreach (var xmlChangePaths in addedFiles.Concat(changedFiles))
                {
                    string path = xmlChangePaths.Key;
                    string localPath = xmlChangePaths.Value;
                    DownloadFile(UpdateURL + path, ConvertChangeUrlPartToPath(localPath));
                    UpdatePercentageRefreshed?.Invoke(this, new UpdatePercentageRefreshedEventArgs(++i / fileChangesCount));
                }

                foreach (var xmlChangePaths in removedFiles)
                {
                    string localPath = xmlChangePaths.Value;
                    File.Delete(ConvertChangeUrlPartToPath(localPath));
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
                Directory.CreateDirectory(dir);

            new WebClient().DownloadFile(url, path);
        }

        public static void UpdateAll(IEnumerable<IUpdateable> updateables, string updateManagerDownloadURL)
        {
            DownloadFile(updateManagerDownloadURL, "UpdateManager.exe");
            string arguments = updateables.Where(x => x.CheckForUpdate()).Aggregate("", (x, y) => x + "\"" + y.XMLURL + "\" \"" + y.UpdateURL + "\" " + y.Version + " ") + "\"" + Process.GetCurrentProcess().ProcessName + ".exe\"";
            Process.Start("UpdateManager.exe", arguments);
        }

        internal static void UpdateAllInternally(IEnumerable<IUpdateable> updateables)
        {
            foreach (IUpdateable updateable in updateables.Where(x => x.CheckForUpdate()))
                GetUpdater(updateable).PerformUpdate();
        }

        public static IEnumerable<string> GetChangeLogFromAll(IEnumerable<IUpdateable> updateables)
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

        public static void PerformUpdate(this IUpdateable updateable, string updateManagerDownloadURL)
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

        public static IEnumerable<string> GetChangeLog(this IUpdateable updateable)
        {
            return GetUpdater(updateable).GetChangeLog();
        }

        #endregion
    }
}
