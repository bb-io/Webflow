using Apps.Webflow.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Webflow.Webhooks.Handlers;

public class SitePublishedWebhookHandler(InvocationContext invocationContext, [WebhookParameter(true)] SiteIdentifier siteRequest) 
    : WebflowWebhookHandler(invocationContext, siteRequest.SiteId)
{
    protected override string EventType => "site_publish";
}