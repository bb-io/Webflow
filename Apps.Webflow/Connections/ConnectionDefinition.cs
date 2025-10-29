using Apps.Webflow.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Webflow.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = ConnectionTypes.OAuth2,
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ClientId){ DisplayName = "Client ID" },
                new(CredsNames.ClientSecret){ DisplayName = "Client secret" }
            }
        },
        new()
        {
            Name = ConnectionTypes.SiteToken,
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.AccessToken){ DisplayName = "API token" }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
    {
        var providers = values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();

        var connectionType = values[nameof(ConnectionPropertyGroup)] switch
        {
            var ct when ConnectionTypes.SupportedConnectionTypes.Contains(ct) => ct,
            _ => throw new Exception($"Unknown connection type: {values[nameof(ConnectionPropertyGroup)]}")
        };

        providers.Add(new AuthenticationCredentialsProvider(CredsNames.ConnectionType, connectionType));
        return providers;
    }
}