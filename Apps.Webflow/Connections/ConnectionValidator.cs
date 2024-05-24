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
        var client = new WebflowClient();
        await client.ExecuteWithErrorHandling(new WebflowRequest("sites", Method.Get,
            authenticationCredentialsProviders));

        return new()
        {
            IsValid = true
        };
    }
}