using Apps.Webflow.Constants;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList("Collections")]
public class CollectionActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Action("Get collection", Description = "Get details of a specific collection")]
    public async Task<FullCollectionEntity> GetCollection([ActionParameter] CollectionRequest collectionRequest)
    {
        var request = new RestRequest($"collections/{collectionRequest.CollectionId}", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<FullCollectionEntity>(request);

        response.CollectionItems = await GetCollectionItems(collectionRequest.CollectionId);
        return response;
    }

    [Action("Create collection", Description = "Create a new collection")]
    public Task<CollectionEntity> CreateCollection([ActionParameter] SiteRequest site,
        [ActionParameter] CreateCollectionRequest input)
    {
        var request = new RestRequest($"sites/{site.SiteId}/collections", Method.Post)
            .WithJsonBody(input, JsonConfig.Settings);
        return Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }

    [Action("Delete collection", Description = "Delete a specific collection")]
    public Task DeleteCollection([ActionParameter] CollectionRequest collectionRequest)
    {
        var request = new RestRequest($"collections/{collectionRequest.CollectionId}", Method.Delete);
        return Client.ExecuteWithErrorHandling(request);
    }

    private Task<List<CollectionItemEntity>> GetCollectionItems(string collectionId)
    {
        var endpoint = $"collections/{collectionId}/items";
        var request = new RestRequest(endpoint, Method.Get);

        return Client.Paginate<CollectionItemEntity>(request);
    }
}