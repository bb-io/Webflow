using RestSharp;
using Apps.Webflow.Constants;
using Apps.Webflow.Invocables;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;

namespace Apps.Webflow.Connections.OAuth;

public class OAuthTokenService(InvocationContext invocationContext) : WebflowInvocable(invocationContext), IOAuth2TokenService
{
    public Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        var tempClientId = values[CredsNames.ClientId];
        var tempClientSecret = values[CredsNames.ClientSecret];

        var parameters = new Dictionary<string, string>
        {
            { "client_id", tempClientId },//ApplicationConstants.ClientId },
            { "client_secret", tempClientSecret },//ApplicationConstants.ClientSecret },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
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