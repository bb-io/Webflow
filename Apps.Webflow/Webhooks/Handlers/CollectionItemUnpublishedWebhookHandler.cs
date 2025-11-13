using Apps.Webflow.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class CollectionItemUnpublishedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteIdentifier siteRequest) 
    : WebflowWebhookHandler(invocationContext, siteRequest.SiteId)
{
    protected override string EventType => "collection_item_unpublished";
}