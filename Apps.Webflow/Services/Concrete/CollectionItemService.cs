using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Webflow.Services.Concrete;

public class CollectionItemService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.CollectionItem;

    public async override Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, DateFilter dateFilter)
    {
        if (string.IsNullOrEmpty(input.CollectionId))
            throw new PluginMisconfigurationException("Please specify collection ID in order to search content items");

        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        var endpoint = $"collections/{input.CollectionId}/items";
        var request = new RestRequest(endpoint, Method.Get);

        var items = await Client.Paginate<CollectionItemEntity, CollectionItemPaginationResponse>(request, r => r.Items);
        IEnumerable<CollectionItemEntity> filtered = FilterHelper.ApplyDateFilters(items, dateFilter);

        if (input.LastPublishedBefore.HasValue)
            filtered = filtered.Where(c => c.LastPublished <= input.LastPublishedBefore);

        if (input.LastPublishedAfter.HasValue)
            filtered = filtered.Where(c => c.LastPublished >= input.LastPublishedAfter);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            filtered = filtered.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        var result = filtered.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Name,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }

    public override async Task<Stream> DownloadContent(string siteId, DownloadContentRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.CollectionId))
            throw new PluginMisconfigurationException("Collection ID is required");

        var collectionRequest = new RestRequest($"collections/{input.CollectionId}", Method.Get);
        var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(collectionRequest);

        var itemEndpoint = $"collections/{input.CollectionId}/items/{input.ContentId}";

        if (!string.IsNullOrEmpty(input.Locale))
        {
            string fetchedCmsLocaleId = await GetCmsLocale(siteId, input.Locale);
            itemEndpoint = itemEndpoint.SetQueryParameter("cmsLocaleId", fetchedCmsLocaleId);
        }

        var itemRequest = new RestRequest(itemEndpoint, Method.Get);
        var item = await Client.ExecuteWithErrorHandling<CollectionItemEntity>(itemRequest);

        var stream = CollectionItemHtmlConverter.ToHtml(item, collection.Fields, siteId, input.CollectionId, input.ContentId);
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public override async Task UploadContent(Stream content, string siteId, UploadContentRequest input)
    {
        var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var itemEndpoint = $"collections/{input.CollectionId}/items/{input.ContentId}";

        string fetchedCmsLocaleId = await GetCmsLocale(siteId, input.Locale!);
        itemEndpoint = itemEndpoint.SetQueryParameter("cmsLocaleId", fetchedCmsLocaleId);

        var itemRequest = new RestRequest(itemEndpoint, Method.Get);
        var item = await Client.ExecuteWithErrorHandling<CollectionItemEntity>(itemRequest);

        var collectionRequest = new RestRequest($"collections/{input.CollectionId}", Method.Get);
        var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(collectionRequest);

        var fieldData = CollectionItemHtmlConverter.ToJson(memoryStream, item.FieldData, collection.Fields);

        var endpoint = $"collections/{input.CollectionId}/items/{input.ContentId}";
        var request = new RestRequest(endpoint, Method.Patch)
            .WithJsonBody(new { fieldData, cmsLocaleId = fetchedCmsLocaleId }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task<string> GetCmsLocale(string siteId, string siteLocaleId)
    {
        var siteRequest = new RestRequest($"/sites/{siteId}", Method.Get);
        var siteEntity = await Client.ExecuteWithErrorHandling<SiteEntity>(siteRequest);

        if (siteEntity.Locales == null)
            throw new PluginApplicationException("Site locales are not available");
        
        if (siteEntity.Locales.Primary?.Id == siteLocaleId)
            return siteEntity.Locales.Primary.CmsLocaleId;

        var secondaryLocale = siteEntity.Locales.Secondary?.FirstOrDefault(x => x.Id == siteLocaleId);
        if (secondaryLocale != null)
            return secondaryLocale.CmsLocaleId;

        var cmsLocale = siteEntity.Locales.Secondary?.FirstOrDefault(x => x.CmsLocaleId == siteLocaleId);
        if (cmsLocale != null)
            return siteLocaleId;

        throw new PluginApplicationException("Can't match the input locale with available collection item locale ID");
    }
}
