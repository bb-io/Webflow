using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.CollectiomItem;
using Apps.Webflow.Models.Response.Pagination;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Net.Mime;

namespace Apps.Webflow.Actions;

[ActionList("Collection items")]
public class CollectionItemActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    [Action("Search collection items", Description = "Search all collection items for a specific collection")]
    public async Task<SearchCollectionItemsResponse> SearchCollectionItems(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionRequest collection,
        [ActionParameter] BasicDateFilter dateFilter,
        [ActionParameter] SearchCollectionItemsRequest input,
        [ActionParameter] LocaleRequest locale)
    {
        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        var endpoint = $"collections/{collection.CollectionId}/items";
        var request = new RestRequest(endpoint, Method.Get);

        if (input.LastPublishedBefore.HasValue)
            request.AddParameter("lastPublished[lte]", input.LastPublishedBefore.Value.ToString("O"));

        if (input.LastPublishedAfter.HasValue)
            request.AddParameter("lastPublished[gte]", input.LastPublishedAfter.Value.ToString("O"));

        if (!string.IsNullOrEmpty(locale.Locale))
        {
            var cmsLocaleId = await LocaleHelper.GetCmsLocaleId(locale.Locale, Client.GetSiteId(site.SiteId), Client);
            request.AddParameter("cmsLocaleId", cmsLocaleId);
        }

        var items = await Client.Paginate<CollectionItemEntity, CollectionItemPaginationResponse>(request, r => r.Items);

        IEnumerable<CollectionItemEntity> filtered = FilterHelper.ApplyDateFilters(items, dateFilter);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.NameContains, r => r.Name);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.SlugContains, r => r.FieldData["slug"]?.ToString());

        return new(filtered);
    }

    [Action("Download collection item", Description = "Download the collection item content")]
    public async Task<DownloadCollectionItemContentResponse> DownloadCollectionItem(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionItemRequest input,
        [ActionParameter] LocaleRequest locale)
    {
        string fileFormat = input.FileFormat ?? MediaTypeNames.Text.Html;

        var service = _factory.GetContentService(ContentTypes.CollectionItem);
        var contentRequest = new DownloadContentRequest
        {
            CollectionId = input.CollectionId,
            ContentId = input.CollectionItemId,
            Locale = locale.Locale,
            FileFormat = fileFormat,
        };

        var stream = await service.DownloadContent(Client.GetSiteId(site.SiteId), contentRequest);

        string fileName = FileHelper.GetDownloadedFileName(fileFormat, input.CollectionItemId, ContentTypes.CollectionItem);
        string contentType = fileFormat == MediaTypeNames.Text.Html ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;

        var file = await fileManagementClient.UploadAsync(stream, contentType, fileName);
        return new(file);
    }

    [Action("Upload collection item", Description = "Update collection item content from a file")]
    public async Task UploadCollectionItem(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UpdateCollectionItemRequest input,
        [ActionParameter] LocaleRequest locale)
    {
        await using var source = await fileManagementClient.DownloadAsync(input.File);
        var bytes = await source.GetByteData();
        await using var stream = new MemoryStream(bytes);

        var service = _factory.GetContentService(ContentTypes.CollectionItem);

        var request = new UploadContentRequest
        {
            CollectionId = input.CollectionId,
            Locale = locale.Locale,
            ContentId = input.CollectionItemId
        };

        await service.UploadContent(stream, Client.GetSiteId(site.SiteId), request);

        // should be reworked to get metadata and pass it here
        if (input.Publish.HasValue && input.Publish.Value)
        {
            var collection = new CollectionRequest { CollectionId = input.CollectionId };
            var publishRequest = new PublishItemRequest { CollectionItemId = input.CollectionItemId };
            await PublishCollectionItem(site, collection, publishRequest, locale);
        }
    }

    [Action("Publish collection item", Description = "Publish a specific collection item")]
    public async Task PublishCollectionItem(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionRequest collection,
        [ActionParameter] PublishItemRequest input,
        [ActionParameter] LocaleRequest locale)
    {
        object payload;

        if (!string.IsNullOrEmpty(locale.Locale))
        {
            var cmsLocaleId = await LocaleHelper.GetCmsLocaleId(locale.Locale, Client.GetSiteId(site.SiteId), Client);
            payload = new
            {
                items = new[]
                {
                    new
                    {
                        id = input.CollectionItemId,
                        cmsLocaleIds = new[] { cmsLocaleId }
                    }
                }
            };
        }
        else
        {
            payload = new
            {
                itemIds = new[] { input.CollectionItemId }
            };
        }

        var request = new RestRequest($"collections/{collection.CollectionId}/items/publish", Method.Post)
            .WithJsonBody(payload, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }
}