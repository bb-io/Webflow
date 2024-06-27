using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class SitePublishedWebhookHandler : WebflowWebhookHandler
{
    protected override string EventType => "site_publish";

    public SitePublishedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteRequest siteRequest) : base(invocationContext,
        siteRequest.SiteId)
    {
    }
}