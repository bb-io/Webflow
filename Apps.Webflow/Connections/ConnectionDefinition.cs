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
            Name = "OAuth2",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ClientId){ DisplayName = "Client ID" },
                new(CredsNames.ClientSecret){ DisplayName = "Client secret" }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values) =>
        values.Select(x =>
                new AuthenticationCredentialsProvider(x.Key, x.Value))
            .ToList();
}