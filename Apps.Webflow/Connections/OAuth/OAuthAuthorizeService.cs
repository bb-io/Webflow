using Apps.Webflow.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Webflow.Connections.OAuth;

public class OAuthAuthorizeService(InvocationContext invocationContext) : BaseInvocable(invocationContext), IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> values)
    {
        var bridgeOauthUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/oauth";

        var tempClientId = values[CredsNames.ClientId];
        var parameters = new Dictionary<string, string>
        {
            { "client_id", tempClientId },//ApplicationConstants.ClientId },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "response_type", "code" },
            { "scope", ApplicationConstants.Scope },
            { "state", values["state"] },
            { "authorization_url", "https://webflow.com/oauth/authorize"},
            { "actual_redirect_uri", InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString() },
        };

        return bridgeOauthUrl.WithQuery(parameters);
    }
}