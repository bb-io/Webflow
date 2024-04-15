using Apps.Webflow.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Invocables;

public class WebflowInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected WebflowClient Client { get; }
    public WebflowInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }
}