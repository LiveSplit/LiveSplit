using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;

namespace LiveSplit.Web.Share;

public class Imgur : IRunUploadPlatform
{
    private const string YOUR_CLIENT_ID = "63e6ae2de8601ef";

    protected static Imgur _Instance = new();

    public static Imgur Instance => _Instance;

    public static readonly Uri BaseUri = new("https://api.imgur.com/");

    protected Imgur() { }

    protected Uri GetUri(string subUri)
    {
        return new Uri(BaseUri, subUri);
    }

    public string PlatformName => "Imgur";

    public string Description
    {
        get
        {
            return "Sharing to Imgur allows you to share a screenshot of "
                + "LiveSplit with a popular web database of image content.";
        }
    }

    public ISettings Settings { get; set; }

    public bool VerifyLogin()
    {
        return true;
    }

    public dynamic UploadImage(Image image, string title = "", string description = "")
    {
        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Png);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var request = (HttpWebRequest)WebRequest.Create(GetUri("3/image"));
        request.Method = "POST";
        request.Headers.Add("Authorization", "Client-ID " + YOUR_CLIENT_ID);

        using (Stream stream = request.GetRequestStream())
        {
            request.ContentType = "multipart/form-data; boundary=AaB03x";
            var writer = new StreamWriter(stream);
            writer.WriteLine("--AaB03x");
            writer.WriteLine("Content-Disposition: form-data; name=\"title\"");
            writer.WriteLine();
            writer.WriteLine(title);
            writer.WriteLine("--AaB03x");
            writer.WriteLine("Content-Disposition: form-data; name=\"description\"");
            writer.WriteLine();
            writer.WriteLine(description);
            writer.WriteLine("--AaB03x");
            writer.WriteLine("Content-Disposition: form-data; name=\"image\"; filename=\"splits.png\"");
            writer.WriteLine("Content-Type: image/png");
            writer.WriteLine();
            writer.Flush();

            memoryStream.CopyTo(stream);

            writer.WriteLine();
            writer.WriteLine("--AaB03x--");
            writer.Flush();
        }

        using (WebResponse response = request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        {
            var reader = new StreamReader(stream);
            string resultString = reader.ReadToEnd();
            dynamic result = JSON.FromString(resultString);

            return result;
        }
    }

    public bool SubmitRun(IRun run, Func<Image> screenShotFunction = null, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
    {
        var titleBuilder = new StringBuilder();

        bool gameNameEmpty = string.IsNullOrEmpty(run.GameName);
        bool categoryEmpty = string.IsNullOrEmpty(run.CategoryName);

        titleBuilder.Append(new RegularTimeFormatter(TimeAccuracy.Seconds).Format(run.Last().PersonalBestSplitTime[method]));
        if (titleBuilder.Length > 0 && (!gameNameEmpty || !categoryEmpty))
        {
            titleBuilder.Append(" in ");
        }

        titleBuilder.Append(run.GameName);
        if (!gameNameEmpty && !categoryEmpty)
        {
            titleBuilder.Append(" - ");
        }

        titleBuilder.Append(run.CategoryName);

        if (screenShotFunction != null)
        {
            Image image = screenShotFunction();
            dynamic result = UploadImage(image, titleBuilder.ToString(), comment);

            string url = "http://imgur.com/" + (string)result.data.id;
            Process.Start(url);
            Clipboard.SetText(url);
        }

        return true;
    }
}
