using LinqToTwitter;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public partial class TwitchOAuthForm : Form
    {
        internal const String RedirectUrl = "http://livesplit.org/twitch/";

        public String AccessToken { get; protected set; }

        public TwitchOAuthForm()
        {
            InitializeComponent();
        }

        void OAuthForm_Load(object sender, EventArgs e)
        {
            OAuthWebBrowser.Navigate(new Uri(
                String.Format(
                "https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri={1}&scope={2}",
                Twitch.ClientId,
                RedirectUrl,
                "channel_editor+chat_login+channel_subscriptions"), UriKind.Absolute));
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
                        ShareSettings.Default.TwitchAccessToken = AccessToken;
                        ShareSettings.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }

                    Action closeAction = () => Close();

                    if (this.InvokeRequired)
                        this.Invoke(closeAction);
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
