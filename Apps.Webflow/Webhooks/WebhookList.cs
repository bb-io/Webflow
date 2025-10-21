using System.Net;
using Apps.Webflow.Extensions;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Webhooks.Handlers;
using Apps.Webflow.Webhooks.Models.Response;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json.Linq;

namespace Apps.Webflow.Webhooks;

[WebhookList]
public class WebhookList(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Webhook("On site published", typeof(SitePublishedWebhookHandler),
        Description = "Triggers when specific site was published")]
    public Task<WebhookResponse<SitePublishedResponse>> OnSitePublished(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<SitePublishedResponse>();

        return Task.FromResult<WebhookResponse<SitePublishedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page created", typeof(PageCreatedWebhookHandler),
        Description = "Triggers when specific page was created")]
    public Task<WebhookResponse<PageCreatedResponse>> OnPageCreated(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<PageCreatedResponse>();

        return Task.FromResult<WebhookResponse<PageCreatedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page deleted", typeof(PageDeletedWebhookHandler),
        Description = "Triggers when specific page was deleted")]
    public Task<WebhookResponse<PageDeletedResponse>> OnPageDeleted(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<PageDeletedResponse>();

        return Task.FromResult<WebhookResponse<PageDeletedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page metadata updated", typeof(PageMetadataUpdatedWebhookHandler),
        Description = "Triggers when specific page metadata was updated")]
    public Task<WebhookResponse<PageUpdatedResponse>> OnPageMetadataUpdated(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<PageUpdatedResponse>();

        return Task.FromResult<WebhookResponse<PageUpdatedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item created", typeof(CollectionItemCreatedWebhookHandler),
        Description = "Triggers when specific collection item was created")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemCreated(WebhookRequest webhookRequest,
        [WebhookParameter] SiteCmsLocaleRequest input)
    {
        var data = webhookRequest.GetPayload<CollectionCreatedWebhookResponse>();

        data.Id ??= (data.FieldData.Descendants().First(x => x is JProperty { Name: "_id" }) as JProperty)!.Value
            .ToString();

        if (input.LocaleId != null && data.FieldData["_locale"]!.ToString() != input.LocaleId)
            return Task.FromResult(new WebhookResponse<CollectionItemResponse>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            });

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item changed", typeof(CollectionItemChangedWebhookHandler),
        Description = "Triggers when specific collection item was changed")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemChanged(WebhookRequest webhookRequest,
        [WebhookParameter] SiteCmsLocaleRequest input)

    {
        var data = webhookRequest.GetPayload<CollectionCreatedWebhookResponse>();

        if (input.LocaleId != null && data.FieldData["_locale"]!.ToString() != input.LocaleId)
            return Task.FromResult(new WebhookResponse<CollectionItemResponse>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            });

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item deleted", typeof(CollectionItemDeletedWebhookHandler),
        Description = "Triggers when specific collection item was deleted")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemDeleted(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item unpublished", typeof(CollectionItemUnpublishedWebhookHandler),
        Description = "Triggers when specific collection item was unpublished")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemUnpublished(
        WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }
}
