using Apps.Webflow.Invocables;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.Connections.OAuth;

public class OAuthTokenService : WebflowInvocable, IOAuth2TokenService
{
    public OAuthTokenService(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "client_id", ApplicationConstants.ClientId },
            { "client_secret", ApplicationConstants.ClientSecret },
            { "code", code },
            { "state", state },
            { "grant_type", "authorization_code" },
        };

        var request = new RestRequest("https://api.webflow.com/oauth/access_token", Method.Post);
        parameters.ToList().ForEach(x => request.AddParameter(x.Key, x.Value));

        return Client.ExecuteWithErrorHandling<Dictionary<string, string>>(request);
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        var parameters = new Dictionary<string, string>
        {
            { "client_id", ApplicationConstants.ClientId },
            { "client_secret", ApplicationConstants.ClientSecret },
            { "access_token", values["access_token"] },
        };

        var request = new RestRequest("https://api.webflow.com/oauth/revoke_authorization", Method.Post);
        parameters.ToList().ForEach(x => request.AddParameter(x.Key, x.Value));

        return Client.ExecuteWithErrorHandling(request);
    }

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(values);
    }

    public bool IsRefreshToken(Dictionary<string, string> values) => false;
}