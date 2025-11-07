using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Apps.Webflow.Models.Request.Content;
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
        [ActionParameter] DateFilter dateFilter,
        [ActionParameter] SearchCollectionItemsRequest input)
    {
        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        var endpoint = $"collections/{collection.CollectionId}/items";
        var request = new RestRequest(endpoint, Method.Get);

        if (input.LastPublishedBefore.HasValue)
            request.AddParameter("lastPublished[lte]", input.LastPublishedBefore.Value.ToString("O"));

        if (input.LastPublishedAfter.HasValue)
            request.AddParameter("lastPublished[gte]", input.LastPublishedAfter.Value.ToString("O"));

        if (!string.IsNullOrEmpty(input.CmsLocaleId))
            request.AddParameter("cmsLocaleId", input.CmsLocaleId);

        var items = await Client.Paginate<CollectionItemEntity, CollectionItemPaginationResponse>(request, r => r.Items);

        IEnumerable<CollectionItemEntity> filtered = FilterHelper.ApplyDateFilters(items, dateFilter);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.NameContains, r => r.Name);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.SlugContains, r => r.FieldData["slug"]?.ToString());

        return new(filtered);
    }

    [Action("Download collection item", Description = "Get content of a specific collection item")]
    public async Task<DownloadCollectionItemContentResponse> DownloadCollectionItem(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionItemRequest input)
    {
        string fileFormat = input.FileFormat ?? MediaTypeNames.Text.Html;

        var service = _factory.GetContentService(ContentTypes.CollectionItem);
        var contentRequest = new DownloadContentRequest
        {
            CollectionId = input.CollectionId,
            ContentId = input.CollectionItemId,
            Locale = input.CmsLocaleId,
            FileFormat = fileFormat,
        };

        var stream = await service.DownloadContent(Client.GetSiteId(site.SiteId), contentRequest);

        string fileName = FileHelper.GetDownloadedFileName(fileFormat, input.CollectionItemId, ContentTypes.CollectionItem);
        string contentType = fileFormat == MediaTypeNames.Text.Html ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;

        var file = await fileManagementClient.UploadAsync(stream, contentType, fileName);
        return new(file);
    }

    [Action("Upload collection item", Description = "Update content of a specific collection item from a file")]
    public async Task UploadCollectionItem(
    [ActionParameter] SiteRequest site,
    [ActionParameter] UpdateCollectionItemRequest input)
    {
        await using var source = await fileManagementClient.DownloadAsync(input.File);
        var bytes = await source.GetByteData();
        await using var stream = new MemoryStream(bytes);

        var service = _factory.GetContentService(ContentTypes.CollectionItem);

        var request = new UploadContentRequest
        {
            CollectionId = input.CollectionId,
            Locale = input.CmsLocaleId,
            ContentId = input.CollectionItemId
        };

        await service.UploadContent(stream, Client.GetSiteId(site.SiteId), request);

        // should be reworked to get metadata and pass it here
        if (input.Publish.HasValue && input.Publish.Value)
        {
            var publishRequest = new PublishItemRequest
            {
                CollectionId = input.CollectionId,
                CollectionItemId = input.CollectionItemId,
                CmsLocaleIds = [input.CmsLocaleId]
            };
            await PublishItem(site, publishRequest);
        }
    }

    [Action("Publish collection item", Description = "Publish a specific collection item")]
    public async Task PublishItem(
        [ActionParameter] SiteRequest site,
        [ActionParameter] PublishItemRequest input)
    {
        var endpoint = $"collections/{input.CollectionId}/items/publish";
        var request = new RestRequest(endpoint, Method.Post)
            .WithJsonBody(new
            {
                items = new[]
                {
                    new
                    {
                        id = input.CollectionItemId,
                        cmsLocaleIds = input.CmsLocaleIds ?? Array.Empty<string>()
                    }
                },
            }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }
}