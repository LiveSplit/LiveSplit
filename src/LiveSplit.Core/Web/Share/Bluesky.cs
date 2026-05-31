using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace LiveSplit.Web.Share;

public class Bluesky : IRunUploadPlatform
{
    public static readonly Uri BaseUri = new("https://bsky.social/");

    public ISettings Settings { get; set; }
    protected static readonly Bluesky _Instance = new();
    public static Bluesky Instance => _Instance;

    protected string dID { get; set; }
    protected string accessJWT { get; set; }

    public string PlatformName => "Bluesky";

    public string Description => """
        Bluesky allows you to share your run with the world. When sharing, a screenshot
        of your splits will automatically be included. After authenticating with your handle
        and a generated app password for the first time, LiveSplit will automatically send the post.
        """;

    protected Bluesky() { }

    protected Uri GetUri(string subUri)
    {
        return new Uri(BaseUri, subUri);
    }

    protected dynamic curlPostData(string subUri, string data, string token = "")
    {
        var request = (HttpWebRequest)WebRequest.Create(GetUri(subUri));
        request.Method = "POST";
        request.Accept = "application/json";
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add($"Authorization: Bearer {token}");
        }

        if (!string.IsNullOrEmpty(data))
        {
            request.ContentType = "application/json; charset=utf-8";
            using var writer = new StreamWriter(request.GetRequestStream());
            writer.Write(data);
        }

        try
        {
            using WebResponse response = request.GetResponse();
            using Stream stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            return JSON.FromString(json);
        }
        catch (WebException ex) when (ex.Response is HttpWebResponse { StatusCode: HttpStatusCode.Unauthorized })
        {
            return null;
        }
    }

    protected dynamic curlPostImage(string subUri, byte[] data, string token = "")
    {
        var request = (HttpWebRequest)WebRequest.Create(GetUri(subUri));
        request.Method = "POST";
        request.Accept = "image/png";
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add($"Authorization: Bearer {token}");
        }

        request.ContentType = "image/png";
        using var writer = new BinaryWriter(request.GetRequestStream());
        writer.Write(data);

        using WebResponse response = request.GetResponse();
        using Stream stream = response.GetResponseStream();
        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();
        return JSON.FromString(json);
    }

    public bool VerifyLogin()
    {

        BlueskyCredentials credentials = WebCredentials.BlueskyCredentials;
        string handle = credentials.Username ?? "";
        string appPassword = credentials.Password ?? "";
        bool rememberPassword = !string.IsNullOrEmpty(appPassword);

        if (!rememberPassword)
        {
            Process.Start("https://bsky.app/settings/app-passwords");
            DialogResult result = LoginBox.Show(
                "Bluesky Authentication",
                "Enter the full name of your Bluesky handle:",
                "Enter the app password you have generated from Bluesky:",
                ref handle,
                ref appPassword,
                ref rememberPassword);
            if (result == DialogResult.Cancel)
            {
                return false;
            }
        }

        string credentialsData = $"{{" +
            $"\"identifier\": \"{handle}\", " +
            $"\"password\": \"{appPassword}\"" +
        $"}}";
        var sessionRes = curlPostData("xrpc/com.atproto.server.createSession", credentialsData);
        if (sessionRes == null)
        {
            return false;
        }

        WebCredentials.BlueskyCredentials = new BlueskyCredentials(handle, rememberPassword ? appPassword : "");
        dID = sessionRes.did;
        accessJWT = sessionRes.accessJwt;

        return true;
    }

    public bool SubmitRun(IRun run, Func<Image> screenShotFunction = null, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
    {

        Image pngImage = screenShotFunction();
        using var memoryStream = new MemoryStream();
        pngImage.Save(memoryStream, ImageFormat.Png);
        var blobRes = curlPostImage("xrpc/com.atproto.repo.uploadBlob", memoryStream.ToArray(), accessJWT);

        string postData = $$"""
            {
                "repo": "{{dID}}",
                "collection": "app.bsky.feed.post",
                "record": {
                    "$type": "app.bsky.feed.post",
                    "text": "{{comment}}",
                    "createdAt": "{{DateTime.UtcNow:o}}",
                    "embed": {
                        "$type": "app.bsky.embed.images",
                        "images": [
                            {
                                "alt": "Image of this user's current splits.",
                                "image": {{blobRes.blob}}
                            }
                        ]
                    }
                }
            }
            """;

        var recordRes = curlPostData("xrpc/com.atproto.repo.createRecord", postData, accessJWT);

        return true;
    }
}
