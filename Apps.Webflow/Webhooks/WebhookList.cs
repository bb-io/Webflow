using Apps.Webflow.Extensions;
using Apps.Webflow.Invocables;
using Apps.Webflow.Webhooks.Handlers;
using Apps.Webflow.Webhooks.Models.Request;
using Apps.Webflow.Webhooks.Models.Response;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.SDK.Blueprints;
using System.Net;

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
    public Task<WebhookResponse<PageCreatedResponse>> OnPageCreated(
        WebhookRequest webhookRequest,
        [WebhookParameter] PageWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<PageCreatedResponse>();

        if (input.TitleContains != null && !data.PageTitle.Contains(input.TitleContains))
            return Preflight<PageCreatedResponse>();

        if (input.PublishedPathContains != null && !data.PublishedPath.Contains(input.PublishedPathContains))
            return Preflight<PageCreatedResponse>();

        return Task.FromResult<WebhookResponse<PageCreatedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page deleted", typeof(PageDeletedWebhookHandler),
        Description = "Triggers when specific page was deleted")]
    public Task<WebhookResponse<PageDeletedResponse>> OnPageDeleted(
        WebhookRequest webhookRequest,
        [WebhookParameter] PageWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<PageDeletedResponse>();

        if (input.TitleContains != null && !data.PageTitle.Contains(input.TitleContains))
            return Preflight<PageDeletedResponse>();

        if (input.PublishedPathContains != null && !data.PublishedPath.Contains(input.PublishedPathContains))
            return Preflight<PageDeletedResponse>();

        return Task.FromResult<WebhookResponse<PageDeletedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On page metadata updated", typeof(PageMetadataUpdatedWebhookHandler),
        Description = "Triggers when specific page metadata was updated")]
    public Task<WebhookResponse<PageUpdatedResponse>> OnPageMetadataUpdated(
        WebhookRequest webhookRequest,
        [WebhookParameter] PageWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<PageUpdatedResponse>();

        if (input.TitleContains != null && !data.PageTitle.Contains(input.TitleContains))
            return Preflight<PageUpdatedResponse>();

        if (input.PublishedPathContains != null && !data.PublishedPath.Contains(input.PublishedPathContains))
            return Preflight<PageUpdatedResponse>();

        return Task.FromResult<WebhookResponse<PageUpdatedResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdated)]
    [Webhook("On collection item created", typeof(CollectionItemCreatedWebhookHandler),
        Description = "Triggers when specific collection item was created")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemCreated(WebhookRequest webhookRequest,
        [WebhookParameter] CollectionItemWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<CollectionWebhookResponse>();

        if (input.LocaleId != null && data.FieldData["_locale"]!.ToString() != input.LocaleId)
            return Preflight<CollectionItemResponse>();

        if (input.CollectionId != null && data.CollectionId != input.CollectionId)
            return Preflight<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item updated", typeof(CollectionItemChangedWebhookHandler),
        Description = "Triggers when specific collection item was changed")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemChanged(WebhookRequest webhookRequest,
        [WebhookParameter] CollectionItemWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<CollectionWebhookResponse>();

        if (input.LocaleId != null && data.FieldData["_locale"]!.ToString() != input.LocaleId)
            return Preflight<CollectionItemResponse>();

        if (input.CollectionId != null && data.CollectionId != input.CollectionId)
            return Preflight<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item deleted", typeof(CollectionItemDeletedWebhookHandler),
        Description = "Triggers when specific collection item was deleted")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemDeleted(WebhookRequest webhookRequest,
        [WebhookParameter] CollectionItemWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<CollectionWebhookResponse>();

        if (input.CollectionId != null && data.CollectionId != input.CollectionId)
            return Preflight<CollectionItemResponse>();

        if (input.LocaleId != null && data.FieldData["_locale"]!.ToString() != input.LocaleId)
            return Preflight<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    [Webhook("On collection item published", typeof(CollectionItemPublishedWebhookHandler),
        Description = "Triggers when specific collection item was published")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemPublished(WebhookRequest webhookRequest,
        [WebhookParameter] CollectionItemWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<CollectionItemPublishedResponse>();

        if (input.LocaleId != null && data.Items.First().FieldData["_locale"]!.ToString() != input.LocaleId)
            return Preflight<CollectionItemResponse>();

        if (input.CollectionId != null && data.Items.First().CollectionId != input.CollectionId)
            return Preflight<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data.Items.First()
        });
    }

    [Webhook("On collection item unpublished", typeof(CollectionItemUnpublishedWebhookHandler),
        Description = "Triggers when specific collection item was unpublished")]
    public Task<WebhookResponse<CollectionItemResponse>> OnCollectionItemUnpublished(WebhookRequest webhookRequest,
        [WebhookParameter] CollectionItemWebhookRequest input)
    {
        var data = webhookRequest.GetPayload<CollectionWebhookResponse>();

        if (input.LocaleId != null && data.FieldData["_locale"]!.ToString() != input.LocaleId)
            return Preflight<CollectionItemResponse>();

        if (input.CollectionId != null && data.CollectionId != input.CollectionId)
            return Preflight<CollectionItemResponse>();

        return Task.FromResult<WebhookResponse<CollectionItemResponse>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }

    private static Task<WebhookResponse<T>> Preflight<T>() where T : class
    {
        return Task.FromResult(new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        });
    }
}
