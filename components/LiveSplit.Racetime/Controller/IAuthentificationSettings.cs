using System.Net;

namespace LiveSplit.Racetime.Controller;

public interface IAuthentificationSettings
{
    string AuthorizationEndpoint { get; }
    string AuthServer { get; }
    string ChallengeMethod { get; }
    string ClientID { get; }
    string ClientSecret { get; }
    string FailureEndpoint { get; }
    IPAddress RedirectAddress { get; }
    int RedirectPort { get; }
    string RevokeEndpoint { get; }
    string Scopes { get; }
    string SuccessEndpoint { get; }
    string TokenEndpoint { get; }
    string UserInfoEndpoint { get; }
}
