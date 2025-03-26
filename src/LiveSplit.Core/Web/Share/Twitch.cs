using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Options;

namespace LiveSplit.Web.Share;

public class Twitch : IRunUploadPlatform
{
    internal const string ClientId = "lkz3x9qaxaeujde1tvq21r8d7cdr40x";

    public static readonly Uri BaseUri = new("https://api.twitch.tv/helix/");

    protected static readonly Twitch _Instance = new();
    public static Twitch Instance => _Instance;

    protected string AccessToken { get; set; }
    public string ChannelName { get; protected set; }
    public string ChannelId { get; protected set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(ChannelId);

    public class TwitchGame
    {
        public string Name;
        public string Id;

        public TwitchGame(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    protected Twitch() { }

    protected Uri GetUri(string subUri)
    {
        return new Uri(BaseUri, subUri);
    }

    public string PlatformName => "Twitch";

    public string Description
=> @"Sharing to Twitch will automatically update your 
stream title and game playing based on the information 
in your splits. Twitch must authenticate with LiveSplit 
the first time that sharing to Twitch is used.";

    public ISettings Settings { get; set; }

    bool IRunUploadPlatform.VerifyLogin()
    {
        return VerifyLogin(true);
    }

    public bool VerifyLogin(bool promptIfNoToken)
    {
        AccessToken = WebCredentials.TwitchAccessToken;

        if (VerifyAccessToken())
        {
            return true;
        }

        if (!promptIfNoToken)
        {
            return false;
        }

        AccessToken = TwitchAccessTokenPrompt.GetAccessToken();

        bool verified = VerifyAccessToken();

        if (verified)
        {
            WebCredentials.TwitchAccessToken = AccessToken;
        }

        return verified;
    }

    protected dynamic curlAbsolute(Uri uri, string method = "GET", string data = "")
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.Headers.Add("Client-ID", ClientId);
        request.Method = method;
        request.Accept = "application/json";
        if (!string.IsNullOrEmpty(AccessToken))
        {
            request.Headers.Add($"Authorization: Bearer {AccessToken}");
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

    protected dynamic curl(string subUri, string method = "GET", string data = "")
    {
        Uri uri = GetUri(subUri);
        return curlAbsolute(uri, method, data);
    }

    public void SetStreamTitleAndGame(string title, TwitchGame game = null)
    {
        curl(
            $"channels?broadcaster_id={HttpUtility.UrlEncode(ChannelId)}",
            "PATCH",
            string.Format("{{" +
                "\"title\":\"{0}\"" +
                (game == null ? "" : ",\"game_id\":\"{1}\"") +
            "}}", title, game?.Id)
        );
    }

    public bool VerifyAccessToken()
    {
        if (string.IsNullOrEmpty(AccessToken))
        {
            return false;
        }

        try
        {
            dynamic verificationInfo = curlAbsolute(new Uri("https://id.twitch.tv/oauth2/validate"));
            ChannelName = verificationInfo.login;
            ChannelId = verificationInfo.user_id;
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return false;
    }

    public void ClearAccessToken()
    {
        ChannelName = null;
        ChannelId = null;
        AccessToken = null;
    }

    public dynamic SearchGame(string name)
    {
        return curl($"search/categories?query={HttpUtility.UrlEncode(name)}");
    }

    public IEnumerable<TwitchGame> FindGame(string name)
    {
        dynamic result = SearchGame(name);
        var games = (IEnumerable<dynamic>)result.data;

        static TwitchGame func(dynamic x)
        {
            return new TwitchGame(x.name, x.id);
        }

        return games.Select(func);
    }

    public dynamic GetVerificationInfo()
    {
        return curl("");
    }

    public Image GetGameBoxArt(string gameName)
    {
        if (!IsLoggedIn)
        {
            if (!VerifyLogin(false))
            {
                return null;
            }
        }

        dynamic url = ((IEnumerable<dynamic>)SearchGame(gameName).data).First().box_art_url;
        dynamic request = WebRequest.Create(url);

        using dynamic response = request.GetResponse();
        using dynamic stream = response.GetResponseStream();
        return Image.FromStream(stream);
    }

    public bool SubmitRun(
        IRun run,
        Func<Image> screenShotFunction = null,
        TimingMethod method = TimingMethod.RealTime,
        string comment = "",
        params string[] additionalParams)
    {
        TwitchGame game = null;

        try
        {
            IEnumerable<TwitchGame> gameList = FindGame(run.GameName);

            game = gameList.First(twitchGame => twitchGame.Name == run.GameName);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            bool resolved = false;

            do
            {
                try
                {
                    var dialog = new TwitchGameResolveDialog(run.GameName);
                    DialogResult result = dialog.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return false;
                    }
                    else if (dialog.Game != null)
                    {
                        game = dialog.Game;
                    }

                    resolved = true;
                }
                catch (Exception exc)
                {
                    Log.Error(exc);
                }
            } while (!resolved);
        }

        SetStreamTitleAndGame(comment, game);

        return true;
    }
}
