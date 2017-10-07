using LinqToTwitter;
using LiveSplit.Options;
using System;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public partial class TwitterOAuthForm : Form
    {
        WebAuthorizer auth;
        public ITwitterAuthorizer Authorizer { get; protected set; }

        public TwitterOAuthForm()
        {
            InitializeComponent();
        }

        void OAuthForm_Load(object sender, EventArgs e)
        {
            auth = new WebAuthorizer
            {
                // Get the ConsumerKey and ConsumerSecret for your app and load them here.
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = Twitter.ConsumerKey,
                    ConsumerSecret = Twitter.ConsumerSecret
                },
                Callback = new Uri("http://livesplit.org/twitter/"),
                // Note: GetPin isn't used here because we've broken the authorization
                // process into two parts: begin and complete
                PerformRedirect = pageLink =>
                    OAuthWebBrowser.Navigate(new Uri(pageLink, UriKind.Absolute))
            };

            auth.BeginAuthorization(new Uri("http://livesplit.org/twitter/"));
        }

        void SubmitPinButton_Click(object sender, EventArgs e)
        {
            
        }

        private void OAuthWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (OAuthWebBrowser.Url.Query.ToLowerInvariant().Contains("oauth_verifier"))
                {
                    auth.CompleteAuthorization(OAuthWebBrowser.Url);
                }
                if (auth.IsAuthorized)
                {
                    Authorizer = auth;

                    // This is how you access credentials after authorization.
                    // The oauthToken and oauthTokenSecret do not expire.
                    // You can use the userID to associate the credentials with the user.
                    // You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
                    // You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
                    // When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
                    //
                    //var credentials = pinAuth.CredentialStore;
                    //string oauthToken = credentials.OAuthToken;
                    //string oauthTokenSecret = credentials.OAuthTokenSecret;
                    //string screenName = credentials.ScreenName;
                    //ulong userID = credentials.UserID;
                    //
                    try
                    {
                        WebCredentials.TwitterOAuthToken = auth.Credentials.OAuthToken;
                        WebCredentials.TwitterAccessToken = auth.Credentials.AccessToken;
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
