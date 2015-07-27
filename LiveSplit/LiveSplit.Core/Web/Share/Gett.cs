using LiveSplit.Model;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public class Gett : IRunUploadPlatform
    {
        protected static Gett _Instance = new Gett();

        public static Gett Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("http://open.ge.tt");
        public static IRunSaver RunSaver = new XMLRunSaver();

        protected Gett() { }

        protected Uri GetUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public string PlatformName
        {
            get { return "Ge.tt"; }
        }

        public string Description
        {
            get
            {
                return "Ge.tt is a useful platform for sharing files "
                + "with the world. You don't need an account. If you're not logged in, the splits "
                + "will be deleted after 30 days, though. "
                + "You can also directly import Ge.tt links with \"Open from URL...\"";
            }
        }

        public ISettings Settings { get; set; }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            yield break;
        }

        public IEnumerable<string> GetGameNames()
        {
            yield break;
        }

        public string GetGameIdByName(string gameName)
        {
            return null;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            yield break;
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            return null;
        }

        public bool VerifyLogin(string username, string password)
        {
            return true;
        }

        public string LoginAnonymous()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://ge.tt/");

            using (var response = request.GetResponse())
            {
                var cookies = response.Headers.GetValues("set-cookie")[0];
                var remaining = cookies.Substring(cookies.IndexOf("accesstoken%22%3A%22") + "accesstoken%22%3A%22".Length);
                var accesstoken = remaining.Substring(0, remaining.IndexOf("%22"));

                return accesstoken;
            }
        }

        public dynamic CreateShare(string accessToken, string title = null)
        {
            var uri = GetUri(string.Format("/1/shares/create?accesstoken={0}", accessToken));
            return JSON.FromUriPost(uri, title != null ? new string[] { "title", title } : new string[0]);
        }

        public dynamic CreateFile(string accessToken, string shareName, string fileName)
        {
            var uri = GetUri(string.Format("/1/files/{0}/create?accesstoken={1}", shareName, accessToken));
            return JSON.FromUriPost(uri, "filename", fileName);
        }

        public void UploadFile(string postUrl, Stream dataStream)
        {
            var request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";

            using (var stream = request.GetRequestStream())
            {
                request.ContentType = "multipart/form-data; boundary=AaB03x";
                var writer = new StreamWriter(stream);
                writer.WriteLine("--AaB03x");
                writer.WriteLine("Content-Disposition: form-data; name=\"blob\"; filename=\"data\"");
                writer.WriteLine("Content-Type: application/octet-stream");
                writer.WriteLine();
                writer.Flush();

                dataStream.CopyTo(stream);

                writer.WriteLine();
                writer.WriteLine("--AaB03x--");
                writer.Flush();
            }

            var response = request.GetResponse();
        }

        public void UploadRun(IRun run, string postUrl)
        {
            using (var memoryStream = new MemoryStream())
            {
                RunSaver.Save(run, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                UploadFile(postUrl, memoryStream);
            }
        }

        public void UploadImage(Image image, string postUrl)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                UploadFile(postUrl, memoryStream);
            }
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            var accessToken = LoginAnonymous();

            var titleBuilder = new StringBuilder();
            titleBuilder.Append("Splits");

            var gameNameEmpty = string.IsNullOrEmpty(run.GameName);
            var categoryEmpty = string.IsNullOrEmpty(run.CategoryName);

            if (!gameNameEmpty || !categoryEmpty)
            {
                titleBuilder.Append(": ");
                titleBuilder.Append(run.GameName);

                if (!categoryEmpty)
                {
                    if (!gameNameEmpty)
                        titleBuilder.Append(" - ");
                    titleBuilder.Append(run.CategoryName);
                }
            }

            var shareData = CreateShare(accessToken, titleBuilder.ToString());
            var shareName = (string)shareData.sharename;

            var fileData = CreateFile(accessToken, shareName, titleBuilder + ".lss");
            var postUrl = (string)fileData.upload.posturl;

            UploadRun(run, postUrl);

            if (screenShotFunction != null)
            {
                fileData = CreateFile(accessToken, shareName, "Screenshot.png");
                postUrl = (string)fileData.upload.posturl;

                var image = screenShotFunction();
                UploadImage(image, postUrl);
            }

            var url = (string)shareData.getturl;
            Process.Start(url);
            Clipboard.SetText(url);

            return true;
        }
    }
}
