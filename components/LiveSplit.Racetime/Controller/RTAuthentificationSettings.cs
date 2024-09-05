using System.Net;

namespace LiveSplit.Racetime.Controller;

internal class RTAuthentificationSettings : IAuthentificationSettings
{
    public virtual string ClientID => Properties.Resources.OAUTH_CLIENTID;
    public virtual string ClientSecret => Properties.Resources.OAUTH_CLIENTSECRET;
    public virtual string AuthServer => Properties.Resources.OAUTH_SERVER;
    public virtual string SuccessEndpoint => Properties.Resources.OAUTH_ENDPOINT_SUCCESS;
    public virtual string FailureEndpoint => Properties.Resources.OAUTH_ENDPOINT_FAILURE;
    public virtual string AuthorizationEndpoint => Properties.Resources.OAUTH_ENDPOINT_AUTHORIZATION;
    public virtual string TokenEndpoint => Properties.Resources.OAUTH_ENDPOINT_TOKEN;
    public virtual string UserInfoEndpoint => Properties.Resources.OAUTH_ENDPOINT_USERINFO;
    public virtual string RevokeEndpoint => Properties.Resources.OAUTH_ENDPOINT_REVOKE;
    public virtual string Scopes => Properties.Resources.OAUTH_SCOPES;
    public virtual IPAddress RedirectAddress => IPAddress.Parse(Properties.Resources.OAUTH_REDIRECT_ADDRESS);
    public virtual int RedirectPort => int.Parse(Properties.Resources.OAUTH_REDIRECT_PORT);
    public virtual string ChallengeMethod => Properties.Resources.OAUTH_CHALLENGE_METHOD;
}
