using Apps.Webflow.Constants;
using Apps.Webflow.Conversion;
using Apps.Webflow.Conversion.CollectionItem;
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
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net.Mime;
using System.Text;

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

        string fileExtension = fileFormat == MediaTypeNames.Text.Html ? "html" : "json";
        string fileName = $"collection_item_{input.CollectionItemId}.{fileExtension}";
        string contentType = fileFormat == MediaTypeNames.Text.Html ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;

        var file = await fileManagementClient.UploadAsync(stream, contentType, fileName);
        return new(file);
    }

    [Action("Upload collection item", Description = "Update content of a specific collection item from a file")]
    public async Task UploadCollectionItem(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UpdateCollectionItemRequest input)
    {
        var fileStream = await fileManagementClient.DownloadAsync(input.File);
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var inputString = Encoding.UTF8.GetString(memoryStream.ToArray());

        if (Xliff2Serializer.IsXliff2(inputString))
        {
            inputString = Transformation.Parse(inputString, input.File.Name).Target().Serialize()
                ?? throw new PluginMisconfigurationException("XLIFF did not contain any files");
        }

        JObject requestObject;
        CollectionItemMetadata metadata;

        if (OriginalJsonValidator.IsJson(inputString))
        {
            requestObject = CollectionItemJsonConverter.ToUploadRequestBody(inputString);
            metadata = CollectionItemJsonConverter.GetMetadata(inputString);
        }
        else
        {
            var itemEndpoint = $"collections/{input.CollectionId}/items/{input.CollectionItemId}";
            var itemRequest = new RestRequest(itemEndpoint, Method.Get);
            var item = await Client.ExecuteWithErrorHandling<CollectionItemEntity>(itemRequest);

            var collectionRequest = new RestRequest($"collections/{input.CollectionId}", Method.Get);
            var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(collectionRequest);
            requestObject = CollectionItemHtmlConverter.ToUploadRequestBody(inputString, item.FieldData, collection.Fields);
            metadata = CollectionItemHtmlConverter.GetMetadata(inputString);
        }

        input.CollectionId ??= metadata.CollectionId ?? 
            throw new PluginMisconfigurationException("Collection ID is missing. Provide it or include it in the HTML file.");
        input.CollectionItemId ??= metadata.CollectionItemId ??
            throw new PluginMisconfigurationException("Collection item ID is missing. Provide it or include it in the HTML file.");
        input.CmsLocaleId ??= metadata.CmsLocaleId ?? 
            throw new PluginMisconfigurationException("CMS locale ID is missing. Provide it or include it in the HTML file.");

        var service = _factory.GetContentService(ContentTypes.CollectionItem);
        var request = new UploadContentRequest 
        { 
            CollectionId = input.CollectionId,
            Locale = input.CmsLocaleId,
            ContentId = input.CollectionItemId
        };

        string fetchedCmsLocaleId = await GetCmsLocale(Client.GetSiteId(site.SiteId), input.CmsLocaleId);

        var endpoint = $"collections/{input.CollectionId}/items/{input.CollectionItemId}";
        var updateRequest = new RestRequest(endpoint, Method.Patch)
            .WithJsonBody(new { requestObject, cmsLocaleId = fetchedCmsLocaleId }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(updateRequest);

        await service.UploadContent(memoryStream, Client.GetSiteId(site.SiteId), request);

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


    private async Task<string> GetCmsLocale(string siteId, string localeId)
    {
        var siteRequest = new RestRequest($"/sites/{siteId}", Method.Get);
        var siteEntity = await Client.ExecuteWithErrorHandling<SiteEntity>(siteRequest);

        if (siteEntity.Locales == null)
            throw new PluginApplicationException("Site locales are not available");

        var cmsLocale = siteEntity.Locales.Secondary?.FirstOrDefault(x => x.CmsLocaleId == localeId);
        if (cmsLocale != null)
            return localeId;

        if (siteEntity.Locales.Primary?.Id == localeId)
            return siteEntity.Locales.Primary.CmsLocaleId;

        var secondaryLocale = siteEntity.Locales.Secondary?.FirstOrDefault(x => x.Id == localeId);
        if (secondaryLocale != null)
            return secondaryLocale.CmsLocaleId;

        throw new PluginApplicationException("Can't match the input locale with available collection item locale ID");
    }
}