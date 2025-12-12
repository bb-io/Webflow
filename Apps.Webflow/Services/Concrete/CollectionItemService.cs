using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.CollectionItem;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Extensions;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities.Collection;
using Apps.Webflow.Models.Entities.CollectionItem;
using Apps.Webflow.Models.Entities.Content;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mime;
using System.Text;

namespace Apps.Webflow.Services.Concrete;

public class CollectionItemService(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.CollectionItem;

    public override async Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, ContentDateFilter dateFilter)
    {
        if (input.CollectionIds is null || input.CollectionIds.Count() == 0)
            throw new PluginMisconfigurationException("Please specify at least one collection ID in order to search content items");

        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        List<CollectionItemEntity> items = [];
        foreach (var collectionId in input.CollectionIds)
        {
            var endpoint = $"collections/{collectionId}/items";
            var request = new RestRequest(endpoint, Method.Get);

            if (input.LastPublishedBefore.HasValue)
                request.AddParameter("lastPublished[lte]", input.LastPublishedBefore.Value.ToString("O"));

            if (input.LastPublishedAfter.HasValue)
                request.AddParameter("lastPublished[gte]", input.LastPublishedAfter.Value.ToString("O"));

            var response = await Client.Paginate<CollectionItemEntity, CollectionItemPaginationResponse>(request, r => r.Items);
            items.AddRange(response);
        }

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

    public override async Task<FileReference> DownloadContent(string siteId, DownloadContentRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.CollectionId))
            throw new PluginMisconfigurationException("Collection ID is required");

        var collectionRequest = new RestRequest($"collections/{input.CollectionId}", Method.Get);
        var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(collectionRequest);

        var itemEndpoint = $"collections/{input.CollectionId}/items/{input.ContentId}";

        if (!string.IsNullOrEmpty(input.Locale))
        {
            var cmsLocaleId = await LocaleHelper.GetCmsLocaleId(input.Locale, siteId, Client);
            itemEndpoint = itemEndpoint.SetQueryParameter("cmsLocaleId", cmsLocaleId);
        }

        var itemRequest = new RestRequest(itemEndpoint, Method.Get);
        var item = await Client.ExecuteWithErrorHandling<CollectionItemEntity>(itemRequest);

        string? slug = input.IncludeSlug == true ? item.FieldData["slug"]?.ToString() : null;
        var metadata = new CollectionItemMetadata(slug);

        Stream outputStream = input.FileFormat switch
        {
            ContentFormats.InteroperableHtml => CollectionItemHtmlConverter.ToHtml(
                item,
                collection.Fields,
                siteId,
                input.CollectionId,
                input.ContentId,
                metadata,
                input.Locale
            ),
            ContentFormats.OriginalJson => CollectionItemJsonConverter.ToJson(
                item,
                input.CollectionId,
                siteId,
                input.Locale
            ),
            _ => throw new PluginMisconfigurationException($"Unsupported output format: {input.FileFormat}")
        }; 
        
        string name = item.FieldData?["name"]?.ToString() ?? input.ContentId;
        string contentFormat = 
            input.FileFormat == ContentFormats.InteroperableHtml 
            ? MediaTypeNames.Text.Html 
            : MediaTypeNames.Application.Json;
        var fileName = FileHelper.GetDownloadedFileName(ContentType, input.ContentId, name, contentFormat);
        
        FileReference fileReference = await fileManagementClient.UploadAsync(outputStream, contentFormat, fileName);
        await outputStream.DisposeAsync();
        return fileReference;
    }

    public override async Task UploadContent(Stream content, string siteId, UploadContentRequest input)
    {
        using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        string fileText = Encoding.UTF8.GetString(memoryStream.ToArray());

        if (JsonHelper.IsJson(fileText))
        {
            await UploadJsonContent(fileText, siteId, input);
        }
        else
        {
            if (Xliff2Serializer.IsXliff2(fileText))
            {
                var htmlFromXliff = Transformation.Parse(fileText, "collectionItem.xlf").Target().Serialize() 
                    ?? throw new PluginMisconfigurationException("XLIFF did not contain valid content.");
                memoryStream.SetLength(0);
                await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(htmlFromXliff));
            }

            memoryStream.Position = 0;
            await UploadHtmlContent(memoryStream, siteId, input);
        }
    }

    private async Task UploadJsonContent(string jsonContent, string siteId, UploadContentRequest input)
    {
        var item = JsonConvert.DeserializeObject<DownloadedCollectionItem>(jsonContent)
            ?? throw new PluginMisconfigurationException("Invalid JSON format.");

        input.CollectionId ??= item.CollectionId;
        input.ContentId ??= item.CollectionItem.Id;
        input.Locale ??= item.Locale;

        await ValidateAndNormalizeInputs(input, siteId);

        if (item.CollectionItem.FieldData == null)
            throw new PluginMisconfigurationException("JSON is missing 'collectionItem.fieldData'.");

        await PatchCollectionItem(input.CollectionId!, input.ContentId!, input.Locale!, item.CollectionItem.FieldData);
    }

    private async Task UploadHtmlContent(Stream htmlStream, string siteId, UploadContentRequest input)
    {
        var doc = new HtmlDocument();
        doc.Load(htmlStream);
        htmlStream.Position = 0;

        input.CollectionId ??= doc.DocumentNode.GetMetaValue("blackbird-collection-id");
        input.ContentId ??= doc.DocumentNode.GetMetaValue("blackbird-collection-item-id");
        input.Locale ??= doc.DocumentNode.GetMetaValue("blackbird-cmslocale");

        await ValidateAndNormalizeInputs(input, siteId);

        var metadata = ParseTranslatableMetadata(doc);

        var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(
            new RestRequest($"collections/{input.CollectionId}", Method.Get));

        var currentItem = await Client.ExecuteWithErrorHandling<CollectionItemEntity>(
            new RestRequest($"collections/{input.CollectionId}/items/{input.ContentId}", Method.Get)
                .AddQueryParameter("cmsLocaleId", input.Locale!));

        if (metadata.Slug is not null)
            currentItem.FieldData["slug"] = metadata.Slug;

        var fieldData = CollectionItemHtmlConverter.ToJson(htmlStream, currentItem.FieldData, collection.Fields);

        await PatchCollectionItem(input.CollectionId!, input.ContentId!, input.Locale!, fieldData);
    }

    private async Task ValidateAndNormalizeInputs(UploadContentRequest input, string siteId)
    {
        if (string.IsNullOrWhiteSpace(input.CollectionId))
            throw new PluginMisconfigurationException("Collection ID is missing. Provide it in the input or file.");
        if (string.IsNullOrWhiteSpace(input.ContentId))
            throw new PluginMisconfigurationException("Collection Item ID is missing. Provide it in the input or file.");
        if (string.IsNullOrWhiteSpace(input.Locale))
            throw new PluginMisconfigurationException("Locale is missing. Provide it in the input or file.");

        input.Locale = await LocaleHelper.GetCmsLocaleId(input.Locale, siteId, Client);
    }

    private static CollectionItemMetadata ParseTranslatableMetadata(HtmlDocument doc)
    {
        var body = doc.DocumentNode.SelectSingleNode("//body");
        string? slug = body?.GetDivText("blackbird-collection-item-slug");
        return new(slug);
    }

    private async Task PatchCollectionItem(string collectionId, string itemId, string cmsLocaleId, object fieldData)
    {
        var request = new RestRequest($"collections/{collectionId}/items/{itemId}", Method.Patch)
            .WithJsonBody(new { fieldData, cmsLocaleId }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }
}
