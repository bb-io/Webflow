using Apps.Webflow.Constants;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.CollectiomItem;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using RestSharp;
using System.Net.Mime;
using System.Text;

namespace Apps.Webflow.Actions;

[ActionList("Collection items")]
public class CollectionItemActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new ContentServicesFactory(invocationContext);

    [Action("Download collection item", Description = "Get content of a specific collection item in HTML format")]
    public async Task<DownloadCollectionItemContentResponse> GetCollectionItemContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionItemRequest input)
    {
        if (string.IsNullOrWhiteSpace(Client.GetSiteId(site.SiteId)))
            throw new PluginMisconfigurationException("Site ID is required.");
        if (string.IsNullOrWhiteSpace(input.CollectionId))
            throw new PluginMisconfigurationException("Collection ID is required.");
        if (string.IsNullOrWhiteSpace(input.CollectionItemId))
            throw new PluginMisconfigurationException("Collection item ID is required.");

        var collection = await GetCollection(input.CollectionId);

        var item = await GetCollectionItem(input.CollectionId, input.CollectionItemId, input.CmsLocaleId);
        var html = CollectionItemHtmlConverter.ToHtml(
            item, 
            collection.Fields, 
            Client.GetSiteId(site.SiteId), 
            input.CollectionId, 
            input.CollectionItemId, 
            item.CmsLocaleId
        );

        var file = await fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"collection_item_{item.Id}.html");
        return new(file);
    }

    [Action("Upload collection item", Description = "Update content of a specific collection item from HTML file")]
    public async Task UpdateCollectionItemContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UpdateCollectionItemRequest input)
    {
        await using var source = await fileManagementClient.DownloadAsync(input.File);
        var html = Encoding.UTF8.GetString(await source.GetByteData());

        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, input.File.Name).Target().Serialize();
            if (html == null) throw new PluginMisconfigurationException("XLIFF did not contain files");
        }

        await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(html));
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(ms);
        ms.Position = 0;

        string? M(string name) =>
            doc.DocumentNode.SelectSingleNode($"//meta[@name='{name}']")
               ?.GetAttributeValue("content", null);

        input.CollectionId ??= M("blackbird-collection-id");
        input.CollectionItemId ??= M("blackbird-collection-item-id");
        input.CmsLocaleId ??= M("blackbird-cmslocale-id");

        if (string.IsNullOrWhiteSpace(input.CollectionId))
            throw new PluginMisconfigurationException("Collection ID is missing. Provide it or include it in the HTML file.");
        if (string.IsNullOrWhiteSpace(input.CollectionItemId))
            throw new PluginMisconfigurationException("Collection item ID is missing. Provide it or include it in the HTML file.");
        if (string.IsNullOrWhiteSpace(input.CmsLocaleId))
            throw new PluginMisconfigurationException("CMS locale ID is missing. Provide it or include it in the HTML file.");

        var service = _factory.GetContentService(ContentTypes.CollectionItem);
        var request = new UploadContentRequest 
        { 
            CollectionId = input.CollectionId,
            Locale = input.CmsLocaleId,
            ContentId = input.CollectionItemId
        };

        await service.UploadContent(ms, Client.GetSiteId(site.SiteId), request);

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

    private Task<CollectionItemEntity> GetCollectionItem(string collectionId, string collectionItemId,
        string? locale = default)
    {
        var endpoint = $"collections/{collectionId}/items/{collectionItemId}";

        if (locale != null)
            endpoint = endpoint.SetQueryParameter("cmsLocaleId", locale);

        var request = new RestRequest(endpoint, Method.Get);

        return Client.ExecuteWithErrorHandling<CollectionItemEntity>(request);
    }

    private Task<CollectionEntity> GetCollection(string collectionId)
    {
        var request = new RestRequest($"collections/{collectionId}", Method.Get);
        return Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }
}