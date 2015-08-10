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
                    index = index + "id=\"api-key\">".Length;
                    var accessToken = html.Substring(index, secondIndex - index);
                    try
                    {
                        ShareSettings.Default.SpeedrunComAccessToken = accessToken;
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
            accessToken = null;
            ShowDialog();
            return accessToken;
        }
    }
}
