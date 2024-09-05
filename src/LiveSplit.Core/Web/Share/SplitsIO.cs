using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;

namespace LiveSplit.Web.Share;

public class SplitsIO : IRunUploadPlatform
{
    protected static readonly SplitsIO _Instance = new();

    public static SplitsIO Instance => _Instance;

    public static readonly Uri BaseUri = new("http://splits.io/");
    public static readonly Uri APIUri = new("https://splits.io/api/v3/");
    public static readonly Uri APIV4Uri = new("https://splits.io/api/v4/");

    public const decimal NoTime = 0.0m;

    protected SplitsIO() { }

    public string PlatformName => "Splits.io";

    public string Description
        => "Splits.io is the best platform for sharing individual "
        + "splits with the world and downloading them from there. "
        + "You can also browse Splits.io with \"Open Splits From Splits.io...\" "
        + "or import splits as a comparison with \"Import Comparison From Splits.io...\". "
        + "Splits downloaded from Splits.io have no data loss, and the splits can be "
        + "downloaded in the format for any timer.";

    public ISettings Settings { get; set; }

    public Uri GetSiteUri(string subUri)
    {
        return new Uri(BaseUri, subUri);
    }

    public Uri GetAPIUri(string subUri)
    {
        return new Uri(APIUri, subUri);
    }

    public Uri GetAPIV4Uri(string subUri)
    {
        return new Uri(APIV4Uri, subUri);
    }

    #region Not supported

    bool IRunUploadPlatform.VerifyLogin()
    {
        return true;
    }

    #endregion

    private static IEnumerable<dynamic> DoPaginatedRequest(Uri uri)
    {
        int page = 1;
        int totalItems = 1;
        int perPage = 1;
        int lastPage = 1;

        do
        {
            var request = WebRequest.Create($"{uri.AbsoluteUri}?page={page}");

            using WebResponse response = request.GetResponse();
            int.TryParse(response.Headers["Total"], NumberStyles.Integer, CultureInfo.InvariantCulture, out totalItems);
            int.TryParse(response.Headers["Per-Page"], NumberStyles.Integer, CultureInfo.InvariantCulture, out perPage);
            lastPage = (int)Math.Ceiling(totalItems / (double)perPage);

            yield return JSON.FromResponse(response);
        } while (page++ < lastPage);
    }

    public IEnumerable<dynamic> SearchGame(string name)
    {
        string escapedName = HttpUtility.UrlPathEncode(name);
        Uri uri = GetAPIUri($"games?search={escapedName}");
        dynamic response = JSON.FromUri(uri);
        return (response.games as IEnumerable<dynamic>) ?? new dynamic[0];
    }

    public dynamic SearchUser(string name)
    {
        try
        {
            string escapedName = HttpUtility.UrlPathEncode(name.ToLowerInvariant());
            Uri uri = GetAPIUri($"users/{escapedName}");
            dynamic response = JSON.FromUri(uri);
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
        Uri uri = GetAPIUri($"games/{gameId}");
        dynamic response = JSON.FromUri(uri);
        return response.game;
    }

    public IEnumerable<dynamic> GetRunsForCategory(int gameId, int categoryId)
    {
        Uri uri = GetAPIUri($"games/{gameId}/categories/{categoryId}/runs");
        IEnumerable<dynamic> pages = DoPaginatedRequest(uri);
        return pages.SelectMany(page => (page.runs as IEnumerable<dynamic>) ?? new dynamic[0]);
    }

    public IEnumerable<dynamic> GetRunsForUser(string userId)
    {
        Uri uri = GetAPIUri($"users/{userId}/runs");
        IEnumerable<dynamic> pages = DoPaginatedRequest(uri);
        return pages.SelectMany(page => (page.runs as IEnumerable<dynamic>) ?? new dynamic[0]);
    }

    public dynamic GetRunById(int runId)
    {
        Uri uri = GetAPIUri($"runs/{runId}");
        dynamic response = JSON.FromUri(uri);
        return response.run;
    }

    public dynamic GetV4RunById(string runId)
    {
        Uri uri = GetAPIV4Uri($"runs/{runId}");
        return JSON.FromUri(uri);
    }

    private void PatchRun(IRun run, string speedrunComId)
    {
        try
        {
            if (string.IsNullOrEmpty(speedrunComId))
            {
                return;
            }

            SpeedrunComSharp.Run speedrunComRun = SpeedrunCom.Client.Runs.GetRun(speedrunComId);
            if (speedrunComRun == null)
            {
                return;
            }

            run.PatchRun(speedrunComRun);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public IRun DownloadRunByPath(string path, bool patchRun)
    {
        Uri uri = GetSiteUri(path);
        return DownloadRunByUri(uri, patchRun);
    }

    public IRun DownloadRunByUri(Uri uri, bool patchRun)
    {
        string id = uri.LocalPath;
        dynamic splitsIORun = GetV4RunById(id);
        string program = splitsIORun.run.program as string;
        Uri downloadUri = GetSiteUri($"{id}/download/{program}");

        var request = WebRequest.Create(downloadUri);

        using WebResponse response = request.GetResponse();
        using Stream stream = response.GetResponseStream();
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var runFactory = new StandardFormatsRunFactory
        {
            Stream = memoryStream,
            FilePath = ""
        };

        IRun run = runFactory.Create(new StandardComparisonGeneratorsFactory());
        if (patchRun)
        {
            PatchRun(run, splitsIORun.run.srdc_id);
        }

        return run;
    }

    public IRun DownloadRunById(int runId, bool patchRun)
    {
        dynamic run = GetRunById(runId);
        dynamic uri = GetSiteUri(run.path);
        return DownloadRunByUri(uri, patchRun);
    }

    public void ClaimWithSpeedrunComRun(string splitsIORunId, string claimToken, string srdcRunId)
    {
        Uri uri = GetAPIV4Uri(
            $"runs/{Uri.EscapeDataString(splitsIORunId)}?claim_token={Uri.EscapeDataString(claimToken)}&srdc_id={Uri.EscapeDataString(srdcRunId)}");

        var request = WebRequest.Create(uri);
        request.Method = "PUT";
        request.GetResponse();
    }

    public string Share(IRun run, Func<Image> screenShotFunction = null, bool claimTokenUri = false)
    {
        string image_url = null;

        if (screenShotFunction != null)
        {
            Image image = screenShotFunction();
            if (image.Width < 280 || image.Height < 150)
            {
                double factor1 = 280.0 / image.Width;
                double factor2 = 150.0 / image.Height;
                double factor = Math.Max(factor1, factor2);
                image = new Bitmap(image, (int)((factor * image.Width) + 0.5), (int)((factor * image.Height) + 0.5));
            }

            dynamic result = Imgur.Instance.UploadImage(image);
            image_url = (string)result.data.link;
        }

        var request = (HttpWebRequest)WebRequest.Create(GetAPIV4Uri("runs").AbsoluteUri);
        request.Method = "POST";
        request.ContentType = "multipart/form-data; boundary=AaB03x";

        using (Stream stream = request.GetRequestStream())
        {
            var writer = new StreamWriter(stream);
            if (image_url != null)
            {
                WriteKeyAndValue(writer, "image_url", image_url);
            }

            writer.WriteLine("--AaB03x--");
            writer.Flush();
        }

        dynamic json;
        string claimUri;
        string publicUri;
        using (WebResponse response = request.GetResponse())
        {
            json = JSON.FromResponse(response);

            claimUri = json.uris.claim_uri;
            publicUri = json.uris.public_uri;
            dynamic presignedRequest = json.presigned_request;

            request = (HttpWebRequest)WebRequest.Create(presignedRequest.uri);
            request.Method = presignedRequest.method;

            dynamic fields = presignedRequest.fields;

            using (Stream stream = request.GetRequestStream())
            {
                request.ContentType = "multipart/form-data; boundary=AaB03x";

                var writer = new StreamWriter(stream);

                WriteKeyAndValue(writer, "key", fields.key);
                WriteKeyAndValue(writer, "policy", fields.policy);
                WriteKeyAndValue(writer, "x-amz-credential", fields["x-amz-credential"]);
                WriteKeyAndValue(writer, "x-amz-algorithm", fields["x-amz-algorithm"]);
                WriteKeyAndValue(writer, "x-amz-date", fields["x-amz-date"]);
                WriteKeyAndValue(writer, "x-amz-signature", fields["x-amz-signature"]);

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

            using WebResponse response2 = request.GetResponse();
            json = JSON.FromResponse(response2);
        }

        return claimTokenUri ? claimUri : publicUri;
    }

    private static void WriteKeyAndValue(TextWriter writer, string key, string value)
    {
        writer.WriteLine("--AaB03x");
        writer.WriteLine("Content-Disposition: form-data; name=\"" + key + "\"");
        writer.WriteLine();
        writer.WriteLine(value);
    }

    public bool SubmitRun(IRun run, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
    {
        string url = Share(run, screenShotFunction, claimTokenUri: true);
        Process.Start(url);
        Clipboard.SetText(url);

        return true;
    }
}
