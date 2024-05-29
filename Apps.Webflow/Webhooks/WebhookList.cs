using Apps.Webflow.Extensions;
using Apps.Webflow.Webhooks.Handlers;
using Apps.Webflow.Webhooks.Models.Response;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json.Linq;

namespace Apps.Webflow.Webhooks;

[WebhookList]
public class WebhookList
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
    public Task<WebhookResponse<PageResponse>> OnPageCreated(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<PageResponse>();

        return Task.FromResult<WebhookResponse<PageResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page deleted", typeof(PageDeletedWebhookHandler),
        Description = "Triggers when specific page was deleted")]
    public Task<WebhookResponse<PageResponse>> OnPageDeleted(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<PageResponse>();

        return Task.FromResult<WebhookResponse<PageResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page metadata updated", typeof(PageMetadataUpdatedWebhookHandler),
        Description = "Triggers when specific page metadata was updated")]
    public Task<WebhookResponse<PageResponse>> OnPageMetadataUpdated(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<PageResponse>();

        return Task.FromResult<WebhookResponse<PageResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item created", typeof(CollectionItemCreatedWebhookHandler),
        Description = "Triggers when specific collection item was created")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemCreated(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<CollectionCreatedWebhookResponse>();

        data.Id ??= (data.FieldData.Descendants().First(x => x is JProperty { Name: "_id" }) as JProperty)!.Value.ToString();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item changed", typeof(CollectionItemChangedWebhookHandler),
        Description = "Triggers when specific collection item was changed")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemChanged(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<CollectionItemResponse>();

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
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemUnpublished(WebhookRequest webhookRequest)
    {
        var data = webhookRequest.GetPayload<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }
}