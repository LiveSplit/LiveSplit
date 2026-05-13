using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Options;

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

    public string Description
=> @"Bluesky allows you to share your run with the world. 
When sharing, a screenshot of your splits will automatically 
be included. When you click share, Bluesky will ask to authenticate 
with LiveSplit. After the authentication, LiveSplit will automatically send the post.";

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

        using WebResponse response = request.GetResponse();
        using Stream stream = response.GetResponseStream();
        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();
        return JSON.FromString(json);
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
        string u = "";
        string p = "";
        string credentialsData = $"{{" +
            $"\"identifier\": \"{u}\", " +
            $"\"password\": \"{p}\"" +
        $"}}";
        var res = curlPostData("xrpc/com.atproto.server.createSession", credentialsData);
        dID = res.did;
        accessJWT = res.accessJwt;

        return true;
    }

    public bool SubmitRun(IRun run, Func<Image> screenShotFunction = null, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
    {
        Image pngImage = screenShotFunction();
        using var memoryStream = new MemoryStream();
        pngImage.Save(memoryStream, ImageFormat.Png);
        var res1 = curlPostImage("xrpc/com.atproto.repo.uploadBlob", memoryStream.ToArray(), accessJWT);

        string postData = $"{{" +
            $"\"repo\": \"{dID}\", " +
            $"\"collection\": \"app.bsky.feed.post\", " +
            $"\"record\": {{" +
                $"\"$type\": \"app.bsky.feed.post\", " +
                $"\"text\": \"{comment}\", " +
                $"\"createdAt\": \"{DateTime.UtcNow.ToString("o")}\", " +
                $"\"embed\": {{" +
                    $"\"$type\": \"app.bsky.embed.images\", " +
                    $"\"images\": [" +
                        $"{{" +
                            $"\"alt\": \"Image of this user's current splits.\", " +
                            $"\"image\": {res1.blob.ToString()}" +
                        $"}}" +
                    $"]" +
                $"}}" +
            $"}}" +
        $"}}";
        var res2 = curlPostData("xrpc/com.atproto.repo.createRecord", postData, accessJWT);

        return true;
    }

}
