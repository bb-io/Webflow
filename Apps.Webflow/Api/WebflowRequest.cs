using Apps.Webflow.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Webflow.Api;

public class WebflowRequest : BlackBirdRestRequest
{
    public WebflowRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds) : base(resource, method, creds)
    {
    }

    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        this.AddHeader("Authorization", $"Bearer {creds.Get(CredsNames.AccessToken).Value}");
    }
}