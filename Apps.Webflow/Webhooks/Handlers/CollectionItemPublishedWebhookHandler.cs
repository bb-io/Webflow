using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class CollectionItemPublishedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteRequest siteRequest) 
    : WebflowWebhookHandler(invocationContext, siteRequest.SiteId)
{
    protected override string EventType => "collection_item_published";
}