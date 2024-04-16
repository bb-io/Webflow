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
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList]
public class CollectionItemActions : WebflowInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public CollectionItemActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Get collection item content as HTML",
        Description = "Get content of a specific collection item in HTML format")]
    public async Task<FileModel> GetCollectionItemContent([ActionParameter] CollectionItemRequest input)
    {
        var collection = await GetCollection(input.CollectionId);

        var item = await GetCollectionItem(input.CollectionId, input.CollectionItemId);
        var html = CollectionItemHtmlConverter.ToHtml(item, collection.Fields);

        return new()
        {
            File = await _fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{item.Id}.html")
        };
    }
    
    [Action("Update collection item content from HTML",
        Description = "Update content of a specific collection item from HTML file")]
    public async Task UpdateCollectionItemContent(
        [ActionParameter] CollectionItemRequest input,
        [ActionParameter] FileModel file)
    {
        var fileStream = await _fileManagementClient.DownloadAsync(file.File);
        
        var item = await GetCollectionItem(input.CollectionId, input.CollectionItemId);
        var fieldData = CollectionItemHtmlConverter.ToJson(fileStream, item.FieldData);

        var endpoint = $"collection/{input.CollectionId}/items/{input.CollectionItemId}";
        var request = new WebflowRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                fieldData
            }, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling<CollectionItemEntity>(request);
    }

    private Task<CollectionItemEntity> GetCollectionItem(string collectionId, string collectionItemId)
    {
        var endpoint = $"collection/{collectionId}/items/{collectionItemId}";
        var request = new WebflowRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<CollectionItemEntity>(request);
    }
    
    private Task<CollectionEntity> GetCollection(string collectionId)
    {
        var request = new WebflowRequest($"collections/{collectionId}", Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }
}