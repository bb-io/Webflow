using Apps.Webflow.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Webflow.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var client = new WebflowClient(authenticationCredentialsProviders);
        await client.ExecuteWithErrorHandling(new RestRequest("sites", Method.Get));

        return new()
        {
            IsValid = true
        };
    }
}