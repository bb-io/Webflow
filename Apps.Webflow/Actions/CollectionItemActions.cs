using System.Net.Mime;
using Apps.Webflow.Api;
using Apps.Webflow.Constants;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList("Collection items")]
public class CollectionItemActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    [Action("Get collection item content as HTML",
        Description = "Get content of a specific collection item in HTML format")]
    public async Task<FileModel> GetCollectionItemContent([ActionParameter] CollectionItemRequest input)
    {
        var collection = await GetCollection(input.CollectionId);

        var item = await GetCollectionItem(input.CollectionId, input.CollectionItemId, input.CmsLocaleId);
        var html = CollectionItemHtmlConverter.ToHtml(item, collection.Fields);

        return new()
        {
            File = await fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{item.Id}.html")
        };
    }

    [Action("Update collection item content from HTML",
        Description = "Update content of a specific collection item from HTML file")]
    public async Task UpdateCollectionItemContent(
        [ActionParameter] CollectionItemRequest input,
        [ActionParameter] FileModel file)
    {
        var fileStream = await fileManagementClient.DownloadAsync(file.File);
        var item = await GetCollectionItem(input.CollectionId, input.CollectionItemId, input.CmsLocaleId);
        var collection = await GetCollection(input.CollectionId);
        var fieldData = CollectionItemHtmlConverter.ToJson(fileStream, item.FieldData, collection.Fields);

        var endpoint = $"collections/{input.CollectionId}/items/{input.CollectionItemId}";
        var request = new WebflowRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                fieldData,
                cmsLocaleId = input.CmsLocaleId,
            }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Publish collection item",
        Description = "Publish a specific collection item")]
    public async Task PublishItem([ActionParameter] PublishItemRequest input)
    {
        var endpoint = $"collections/{input.CollectionId}/items/publish";
        var request = new WebflowRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(new
            {
                itemIds = new[] { input.CollectionItemId },
            }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<CollectionItemEntity> GetCollectionItem(string collectionId, string collectionItemId,
        string? locale = default)
    {
        var endpoint = $"collections/{collectionId}/items/{collectionItemId}";

        if (locale != null)
            endpoint = endpoint.SetQueryParameter("cmsLocaleId", locale);

        var request = new WebflowRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<CollectionItemEntity>(request);
    }

    private Task<CollectionEntity> GetCollection(string collectionId)
    {
        var request = new WebflowRequest($"collections/{collectionId}", Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }
}