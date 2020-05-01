using LiveSplit.Options;
using System;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public partial class TwitchOAuthForm : Form
    {
		// ==EXTREMELY IMPORTANT NOTE==
		// We still need to implement the code from https://www.codeproject.com/Articles/793687/Configuring-the-Emulation-Mode-of-an-Internet-Expl to add the "LiveSplit.exe" Registry key
		// so that we can access Twitch.tv outside of Compatibility View! If we do not do this, users will not be able to access Twitch via LiveSplit at all!

		internal const string RedirectUrl = "https://livesplit.org/twitch/";

        public string AccessToken { get; protected set; }

        public TwitchOAuthForm()
        {
            InitializeComponent();
        }

		void OAuthForm_Load(object sender, EventArgs e)
        {
			OAuthWebBrowser.Navigate(new Uri(string.Format(
				"https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri={1}&scope={2}",
				Twitch.ClientId,
				RedirectUrl,
				"channel_editor+chat_login+channel_subscriptions"
			), UriKind.Absolute));
		}

        private void OAuthWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                var url = OAuthWebBrowser.Url.Fragment.ToLowerInvariant();
                if (url.Contains("access_token"))
                {
                    var cutoff = url.Substring(url.IndexOf("access_token") + "access_token=".Length);
                    AccessToken = cutoff.Substring(0, cutoff.IndexOf("&"));

                    try
                    {
                        WebCredentials.TwitchAccessToken = AccessToken;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }

                    Action closeAction = () => Close();

                    if (InvokeRequired)
                        Invoke(closeAction);
                    else
                        closeAction();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
