using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class PageDeletedWebhookHandler : WebflowWebhookHandler
{
    protected override string EventType => "page_deleted";

    public PageDeletedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteRequest siteRequest) : base(invocationContext,
        siteRequest.SiteId)
    {
    }
}