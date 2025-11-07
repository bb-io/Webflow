using Apps.Webflow.Constants;
using Apps.Webflow.Conversion;
using Apps.Webflow.Conversion.CollectionItem;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using Newtonsoft.Json;
using RestSharp;
using System.Text;

namespace Apps.Webflow.Services.Concrete;

public class CollectionItemService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.CollectionItem;

    public async override Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, ContentDateFilter dateFilter)
    {
        if (string.IsNullOrEmpty(input.CollectionId))
            throw new PluginMisconfigurationException("Please specify collection ID in order to search content items");

        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        var endpoint = $"collections/{input.CollectionId}/items";
        var request = new RestRequest(endpoint, Method.Get);

        if (input.LastPublishedBefore.HasValue)
            request.AddParameter("lastPublished[lte]", input.LastPublishedBefore.Value.ToString("O"));

        if (input.LastPublishedAfter.HasValue)
            request.AddParameter("lastPublished[gte]", input.LastPublishedAfter.Value.ToString("O"));

        var items = await Client.Paginate<CollectionItemEntity, CollectionItemPaginationResponse>(request, r => r.Items);
        IEnumerable<CollectionItemEntity> filtered = FilterHelper.ApplyDateFilters(items, dateFilter);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.NameContains, r => r.Name);

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

        Stream outputStream = input.FileFormat switch
        {
            "text/html" => CollectionItemHtmlConverter.ToHtml(
                item,
                collection.Fields,
                siteId,
                input.CollectionId,
                input.ContentId,
                item.CmsLocaleId
            ),
            "original" => CollectionItemJsonConverter.ToJson(
                item,
                input.CollectionId,
                siteId
            ),
            _ => throw new PluginMisconfigurationException($"Unsupported output format: {input.FileFormat}")
        };

        var memoryStream = new MemoryStream();
        await outputStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public override async Task UploadContent(Stream content, string siteId, UploadContentRequest input)
    {
        using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        string fileText;
        using (var reader = new StreamReader(memoryStream, Encoding.UTF8, leaveOpen: true))
            fileText = await reader.ReadToEndAsync();

        memoryStream.Position = 0;

        if (OriginalJsonValidator.IsJson(fileText))
        {
            await UploadJsonContent(fileText, siteId, input);
            return;
        }
        else
        {
            if (Xliff2Serializer.IsXliff2(fileText))
            {
                fileText = Transformation.Parse(fileText, "collectionItem.xlf").Target().Serialize();
                if (fileText == null)
                    throw new PluginMisconfigurationException("XLIFF did not contain files");

                memoryStream.SetLength(0);
                await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(fileText));
                memoryStream.Position = 0;
            }

            await UploadHtmlContent(memoryStream, siteId, input);
        }
    }

    private async Task UploadHtmlContent(Stream stream, string siteId, UploadContentRequest input)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(stream);
        stream.Position = 0;

        string? Meta(string name) =>
            doc.DocumentNode.SelectSingleNode($"//meta[@name='{name}']")
                ?.GetAttributeValue("content", "");

        input.CollectionId ??= Meta("blackbird-collection-id");
        input.ContentId ??= Meta("blackbird-collection-item-id");
        input.Locale ??= Meta("blackbird-cmslocale-id");

        if (string.IsNullOrWhiteSpace(input.CollectionId))
            throw new PluginMisconfigurationException("Collection ID is missing. Provide it or include it in the HTML file.");
        if (string.IsNullOrWhiteSpace(input.ContentId))
            throw new PluginMisconfigurationException("Collection item ID is missing. Provide it or include it in the HTML file.");
        if (string.IsNullOrWhiteSpace(input.Locale))
            throw new PluginMisconfigurationException("CMS locale ID is missing. Provide it or include it in the HTML file.");

        var itemEndpoint = $"collections/{input.CollectionId}/items/{input.ContentId}";
        string fetchedCmsLocaleId = await GetCmsLocale(siteId, input.Locale!);
        itemEndpoint = itemEndpoint.SetQueryParameter("cmsLocaleId", fetchedCmsLocaleId);

        var itemRequest = new RestRequest(itemEndpoint, Method.Get);
        var item = await Client.ExecuteWithErrorHandling<CollectionItemEntity>(itemRequest);

        var collectionRequest = new RestRequest($"collections/{input.CollectionId}", Method.Get);
        var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(collectionRequest);

        var fieldData = CollectionItemHtmlConverter.ToJson(stream, item.FieldData, collection.Fields);

        var endpoint = $"collections/{input.CollectionId}/items/{input.ContentId}";
        var request = new RestRequest(endpoint, Method.Patch)
            .WithJsonBody(new { fieldData, cmsLocaleId = fetchedCmsLocaleId }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task UploadJsonContent(string contentText, string siteId, UploadContentRequest input)
    {
        var item = JsonConvert.DeserializeObject<DownloadedCollectionItem>(contentText)
            ?? throw new PluginMisconfigurationException("Invalid JSON format for collection item upload.");

        var collectionId = input.CollectionId ?? item.CollectionId;
        var itemId = input.ContentId ?? item.CollectionItem.Id;
        var cmsLocaleId = input.Locale ?? item.CollectionItem.CmsLocaleId;

        if (string.IsNullOrWhiteSpace(collectionId))
            throw new PluginMisconfigurationException("Missing 'collectionId' in JSON upload.");
        if (string.IsNullOrWhiteSpace(itemId))
            throw new PluginMisconfigurationException("Missing 'collectionItem.id' in JSON upload.");
        if (string.IsNullOrWhiteSpace(cmsLocaleId))
            throw new PluginMisconfigurationException("Missing 'collectionItem.cmsLocaleId' in JSON upload.");
        if (item.CollectionItem.FieldData == null)
            throw new PluginMisconfigurationException("Missing 'collectionItem.fieldData' in JSON upload.");

        string fetchedCmsLocaleId = await GetCmsLocale(siteId, cmsLocaleId);

        var endpoint = $"collections/{collectionId}/items/{itemId}".SetQueryParameter("cmsLocaleId", fetchedCmsLocaleId);
        var request = new RestRequest(endpoint, Method.Patch)
            .WithJsonBody(new
            {
                fieldData = item.CollectionItem.FieldData,
                cmsLocaleId = fetchedCmsLocaleId
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
        
        if (siteEntity.Locales.Primary?.Id == localeId || siteEntity.Locales.Primary?.CmsLocaleId == localeId)
            return siteEntity.Locales.Primary.CmsLocaleId;

        var secondaryLocale = siteEntity.Locales.Secondary?.FirstOrDefault(x => x.Id == localeId);
        if (secondaryLocale != null)
            return secondaryLocale.CmsLocaleId;

        throw new PluginApplicationException("Can't match the input locale with available collection item locale ID");
    }
}
