using LiveSplit.Model;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;
using LiveSplit.Updates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;
using LiveSplit.Model.RunFactories;
using LiveSplit.Model.Comparisons;

namespace LiveSplit.Web.Share
{
    public class SplitsIO : IRunUploadPlatform
    {
        protected static SplitsIO _Instance = new SplitsIO();

        public static SplitsIO Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("http://splits.io/");
        public static readonly Uri APIUri = new Uri("https://splits.io/api/v3/");

        public const decimal NoTime = 0.0m;

        protected SplitsIO() { }

        public string PlatformName
        {
            get { return "Splits.io"; }
        }

        public string Description
        {
            get 
            {
                return "Splits.io is the best platform for sharing individual "
                + "splits with the world and downloading them from there. "
                + "You can also browse Splits.io with \"Open Splits From Splits.io...\" "
                + "or import splits as a comparison with \"Import Comparison From Splits.io...\". "
                + "Splits downloaded from Splits.io have not data loss, and the splits can be "
                + "downloaded in the format for any timer.";
            }
        }

        public ISettings Settings { get; set; }

        public Uri GetSiteUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public Uri GetAPIUri(string subUri)
        {
            return new Uri(APIUri, subUri);
        }

        #region Not supported

        IEnumerable<ASUP.IdPair> IRunUploadPlatform.GetGameList()
        {
            yield break;
        }

        IEnumerable<string> IRunUploadPlatform.GetGameNames()
        {
            yield break;
        }

        string IRunUploadPlatform.GetGameIdByName(string gameName)
        {
            return string.Empty;
        }

        IEnumerable<ASUP.IdPair> IRunUploadPlatform.GetGameCategories(string gameId)
        {
            yield break;
        }

        string IRunUploadPlatform.GetCategoryIdByName(string gameId, string categoryName)
        {
            return string.Empty;
        }

        bool IRunUploadPlatform.VerifyLogin(string username, string password)
        {
            return true;
        }

        #endregion

        public IEnumerable<dynamic> SearchGame(string name)
        {
            var escapedName = HttpUtility.UrlPathEncode(name);
            var uri = GetAPIUri(string.Format("games?search={0}", escapedName));
            var response = JSON.FromUri(uri);
            return (response.games as IEnumerable<dynamic>) ?? new dynamic[0];
        }

        public dynamic SearchUser(string name)
        {
            try
            {
                var escapedName = HttpUtility.UrlPathEncode(name.ToLowerInvariant());
                var uri = GetAPIUri(string.Format("users/{0}", escapedName));
                var response = JSON.FromUri(uri);
                return response.user;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public dynamic GetGameById(int gameId)
        {
            var uri = GetAPIUri(string.Format("games/{0}", gameId));
            var response = JSON.FromUri(uri);
            return response.game;
        }

        public IEnumerable<dynamic> GetRunsForCategory(int gameId, int categoryId)
        {
            var uri = GetAPIUri(string.Format("games/{0}/categories/{1}/runs", gameId, categoryId));
            var response = JSON.FromUri(uri);
            return (response.runs as IEnumerable<dynamic>) ?? new dynamic[0];
        }

        public IEnumerable<dynamic> GetRunsForUser(string userId)
        {
            var uri = GetAPIUri(string.Format("users/{0}/runs", userId));
            var response = JSON.FromUri(uri);
            return (response.runs as IEnumerable<dynamic>) ?? new dynamic[0];
        }

        public dynamic GetRunById(int runId)
        {
            var uri = GetAPIUri(string.Format("runs/{0}", runId));
            var response = JSON.FromUri(uri);
            return response.run;
        }

        public IRun DownloadRunByPath(string path)
        {
            var uri = GetSiteUri(path);
            return DownloadRunByUri(uri);
        }

        public IRun DownloadRunByUri(Uri uri)
        {
            var downloadUri = GetSiteUri(string.Format("{0}/download/livesplit", uri.LocalPath));

            var request = WebRequest.Create(downloadUri);
            using (var stream = request.GetResponse().GetResponseStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var runFactory = new XMLRunFactory();

                    runFactory.Stream = memoryStream;
                    runFactory.FilePath = null;

                    return runFactory.Create(new StandardComparisonGeneratorsFactory());
                }
            }
        }

        public IRun DownloadRunById(int runId)
        {
            var run = GetRunById(runId);
            var uri = GetSiteUri(run.path);
            return DownloadRunByUri(uri);
        }

        public string Share(IRun run, Func<Image> screenShotFunction = null)
        {
            string image_url = null;

            if (screenShotFunction != null)
            {
                var image = screenShotFunction();
                if (image.Width < 280 || image.Height < 150)
                {
                    var factor1 = 280.0 / image.Width;
                    var factor2 = 150.0 / image.Height;
                    var factor = Math.Max(factor1, factor2);
                    image = new Bitmap(image, (int)(factor * image.Width + 0.5), (int)(factor * image.Height + 0.5));
                }
                var result = Imgur.Instance.UploadImage(image);
                image_url = (string)result.data.link;
            }

            var request = (HttpWebRequest)WebRequest.Create("http://splits.io/api/v2/runs");
            request.Method = "POST";
            request.Host = "splits.io";
            request.UserAgent = "LiveSplit/" + UpdateHelper.Version.ToString();
            using (var stream = request.GetRequestStream())
            {
                request.ContentType = "multipart/form-data; boundary=AaB03x";
                request.Referer = "http://splits.io/upload/fallback";
                request.Headers.Add("Origin", "http://splits.io");

                var writer = new StreamWriter(stream);

                if (image_url != null)
                {
                    writer.WriteLine("--AaB03x");
                    writer.WriteLine("Content-Disposition: form-data; name=\"image_url\"");
                    writer.WriteLine();
                    writer.WriteLine(image_url);
                }

                writer.WriteLine("--AaB03x");
                writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"splits\"");
                writer.WriteLine("Content-Type: application/octet-stream");
                writer.WriteLine();
                writer.Flush();

                new XMLRunSaver().Save(run, stream);

                writer.WriteLine();
                writer.WriteLine("--AaB03x--");
                writer.Flush();
            }

            var response = request.GetResponse();
            var json = JSON.FromUri(new Uri(response.Headers["Location"]));

            var url = "http://splits.io" + json.run.path;
            return url;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            var url = Share(run, screenShotFunction);
            Process.Start(url);
            Clipboard.SetText(url);

            return true;
        }
    }
}
