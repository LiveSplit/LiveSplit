using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Web.Share;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class URLRunImporter : IRunImporter
    {
        private static IRun LoadRunFromURL(string url, Form form = null)
        {
            var runFactory = new StandardFormatsRunFactory();
            var comparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

            //var uri = new Uri(url);
            Uri uri = null;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(form, "Not an URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var host = uri.Host.ToLowerInvariant();

            try
            {
                // Supports opening from URLs such as https://one.livesplit.org/#/splits-io/46qh
                int liveSplitOneSplitsIOIndex = url.IndexOf("#/splits-io/");
                if (liveSplitOneSplitsIOIndex != -1)
                {
                    // 12 is the length of #/splits-io/
                    url = "https://splits.io/" + url.Substring(liveSplitOneSplitsIOIndex + 12);
                }

                if (host == "splits.io")
                {
                    return SplitsIO.Instance.DownloadRunByUri(uri, true);
                }
                if (host == "www.speedrun.com" || host == "speedrun.com")
                {
                    var speedrunComRun = SpeedrunCom.Client.Runs.GetRunFromSiteUri(url);
                    if (speedrunComRun != null && speedrunComRun.SplitsAvailable)
                    {
                        var run = speedrunComRun.GetRun();
                        run.PatchRun(speedrunComRun);
                        return run;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(form, "The splits file couldn't be downloaded from spilts.io or speedrun.com", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            WebResponse response = null;

            try
            {
                var request = WebRequest.Create(uri);
                response = request.GetResponse();
            }
            catch (WebException webException)
            {
                Log.Info("WebEx is caught");
                var errorResponse = webException.Response as HttpWebResponse;

                if (errorResponse == null)
                {
                    Log.Error("errorResponse not of type HttpWebResponse");
                    return null;
                }

                if (errorResponse.Headers["WWW-Authenticate"].IndexOf("Basic") != -1)
                {
                    Log.Info("WebEx is tested for 401 on basic auth");
                    var requestAuth = WebRequest.Create(uri);

                    String uname = "";
                    String pw = "";
                    Log.Info("basic auth credentials are required");

                    if (DialogResult.OK != BasicAuthInputBox.Show(form, "Get Auth credentials", "Username:", "Password:", ref uname, ref pw))
                    {
                        return null;
                    }

                    requestAuth.UseDefaultCredentials = false;
                    requestAuth.Credentials = new NetworkCredential(uname, pw);
                    requestAuth.PreAuthenticate = true;
                    try
                    {
                        response = requestAuth.GetResponse();
                    }
                    catch (WebException webEx)
                    {
                        Log.Error(webEx);
                        MessageBox.Show(form, "Authentication failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
            }

            //setting stream to responseStream might create an exception
            using (var stream = response.GetResponseStream())
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                runFactory.Stream = memoryStream;
                runFactory.FilePath = "";

                try
                {
                    return runFactory.Create(comparisonGeneratorsFactory);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(form, "The selected file was not recognized as a splits file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return null;
            }
        }

        public IRun Import(Form form = null)
        {
            string url = null;
            if (DialogResult.OK == InputBox.Show(form, "Open Splits from URL", "URL:", ref url))
            {
                return LoadRunFromURL(url, form);
            }
            return null;
        }

        public string ImportAsComparison(IRun run, Form form = null)
        {
            string url = null;
            string name = null;
            if (DialogResult.OK == InputBox.Show(form, "Import Comparison from URL", "Name:", "URL:", ref name, ref url))
            {
                var imported = LoadRunFromURL(url, form);
                return run.AddComparisonWithNameInput(imported, name, form);
            }
            return null;
        }
    }
}
