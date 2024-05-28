using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class CollectionItemDeletedWebhookHandler : WebflowWebhookHandler
{
    protected override string EventType => "collection_item_deleted";

    public CollectionItemDeletedWebhookHandler(InvocationContext invocationContext, [WebhookParameter] SiteRequest siteRequest) : base(invocationContext,
        siteRequest.SiteId)
    {
    }
}