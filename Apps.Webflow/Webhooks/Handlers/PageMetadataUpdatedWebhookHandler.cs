using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class PageMetadataUpdatedWebhookHandler : WebflowWebhookHandler
{
    protected override string EventType => "page_metadata_updated";

    public PageMetadataUpdatedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteLocaleRequest siteRequest) : base(invocationContext,
        siteRequest.SiteId)
    {
    }
}