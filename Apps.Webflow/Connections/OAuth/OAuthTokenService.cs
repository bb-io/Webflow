using Apps.Webflow.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Webflow.Connections.OAuth;

public class OAuthTokenService(InvocationContext invocationContext) : BaseInvocable(invocationContext), IOAuth2TokenService
{
    public async Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        var tempClientId = values[CredsNames.ClientId];
        var tempClientSecret = values[CredsNames.ClientSecret];

        var parameters = new Dictionary<string, string>
        {
            { "client_id", tempClientId },
            { "client_secret", tempClientSecret },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "code", code },
            { "state", state },
            { "grant_type", "authorization_code" },
        };

        using var client = new HttpClient();
        using var content = new FormUrlEncodedContent(parameters);
        using var response = await client.PostAsync("https://api.webflow.com/oauth/access_token", content, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error requesting token: {response.StatusCode} - {responseContent}");
        }

        return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent!)!;
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

        var restClient = new RestClient();
        return restClient.ExecuteAsync(request);
    }

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(values);
    }

    public bool IsRefreshToken(Dictionary<string, string> values) => false;
}