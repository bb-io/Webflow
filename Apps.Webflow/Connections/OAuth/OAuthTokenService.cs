using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;

namespace Apps.Webflow.Connections.OAuth;

public class OAuthTokenService : IOAuth2TokenService
{
    public Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }
}