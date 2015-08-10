using LiveSplit.Options;
using System;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public partial class SpeedrunComOAuthForm : Form, ISpeedrunComAuthenticator
    {
        private string accessToken;

        public SpeedrunComOAuthForm()
        {
            InitializeComponent();
        }

        void OAuthForm_Load(object sender, EventArgs e)
        {
            OAuthWebBrowser.Navigate(new Uri("http://www.speedrun.com/api/auth"));
        }

        private void OAuthWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                var html = OAuthWebBrowser.DocumentText;
                var index = html.IndexOf("id=\"api-key\">");
                var secondIndex = html.IndexOf("</code");
                if (index >= 0 && secondIndex >= 0)
                {
                    var accessToken = html.Substring(index + "id=\"api-key\">".Length, secondIndex - index);
                }

                var url = OAuthWebBrowser.Url.Fragment.ToLowerInvariant();
                if (url.Contains("access_token"))
                {
                    var cutoff = url.Substring(url.IndexOf("access_token") + "access_token=".Length);
                    accessToken = cutoff.Substring(0, cutoff.IndexOf("&"));

                    try
                    {
                        ShareSettings.Default.TwitchAccessToken = ;
                        ShareSettings.Default.Save();
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

        public string GetAccessToken()
        {
            var result = ShowDialog();
        }
    }
}
