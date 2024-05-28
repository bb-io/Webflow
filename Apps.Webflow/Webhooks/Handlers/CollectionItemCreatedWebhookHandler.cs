using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class CollectionItemCreatedWebhookHandler : WebflowWebhookHandler
{
    protected override string EventType => "collection_item_created";

    public CollectionItemCreatedWebhookHandler(InvocationContext invocationContext, [WebhookParameter] SiteRequest siteRequest) : base(invocationContext,
        siteRequest.SiteId)
    {
    }
}