using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Web.Share;

namespace LiveSplit.Model.RunImporters;

public class URLRunImporter : IRunImporter
{
    private static IRun LoadRunFromURL(string url, Form form = null)
    {
        try
        {
            var runFactory = new StandardFormatsRunFactory();
            var comparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

            // Supports opening from URLs such as https://one.livesplit.org/#/splits-io/46qh
            int liveSplitOneSplitsIOIndex = url.IndexOf("#/splits-io/");
            if (liveSplitOneSplitsIOIndex != -1)
            {
                // 12 is the length of #/splits-io/
                url = "https://splits.io/" + url[(liveSplitOneSplitsIOIndex + 12)..];
            }

            var uri = new Uri(url);
            string host = uri.Host.ToLowerInvariant();
            if (host == "splits.io")
            {
                return SplitsIO.Instance.DownloadRunByUri(uri, true);
            }

            if (host is "www.speedrun.com" or "speedrun.com")
            {
                SpeedrunComSharp.Run speedrunComRun = SpeedrunCom.Client.Runs.GetRunFromSiteUri(url);
                if (speedrunComRun != null && speedrunComRun.SplitsAvailable)
                {
                    IRun run = speedrunComRun.GetRun();
                    run.PatchRun(speedrunComRun);
                    return run;
                }
            }

            var request = WebRequest.Create(uri);

            using WebResponse response = request.GetResponse();
            using Stream stream = response.GetResponseStream();
            using var memoryStream = new MemoryStream();
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
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            MessageBox.Show(form, "The splits file couldn't be downloaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return null;
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
            IRun imported = LoadRunFromURL(url, form);
            return run.AddComparisonWithNameInput(imported, name, form);
        }

        return null;
    }
}
