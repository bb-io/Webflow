using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Webflow.Connections.OAuth;

public class OAuthAuthorizeService : BaseInvocable, IOAuth2AuthorizeService
{
    public OAuthAuthorizeService(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public string GetAuthorizationUrl(Dictionary<string, string> values)
    {
        var bridgeOauthUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/oauth";
        var parameters = new Dictionary<string, string>
        {
            { "client_id", ApplicationConstants.ClientId },
            { "response_type", "code" },
            { "scope", ApplicationConstants.Scope },
            { "state", values["state"] },
            { "authorization_url", "https://webflow.com/oauth/authorize"},
            { "actual_redirect_uri", InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString() },
        };

        return bridgeOauthUrl.WithQuery(parameters);
    }
}