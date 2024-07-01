using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class CollectionItemChangedWebhookHandler : WebflowWebhookHandler
{
    protected override string EventType => "collection_item_changed";

    public CollectionItemChangedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteCmsLocaleRequest siteRequest) : base(invocationContext,
        siteRequest.SiteId)
    {
    }
}