using Apps.Webflow.Api;
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

[ActionList]
public class CollectionActions : WebflowInvocable
{
    public CollectionActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get collection", Description = "Get details of a specific collection")]
    public Task<CollectionEntity> GetCollection([ActionParameter] CollectionRequest collectionRequest)
    {
        var request = new WebflowRequest($"collections/{collectionRequest.CollectionId}", Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }

    [Action("Create collection", Description = "Create a new collection")]
    public Task<CollectionEntity> CreateCollection([ActionParameter] SiteRequest site,
        [ActionParameter] CreateCollectionRequest input)
    {
        var request = new WebflowRequest($"sites/{site.SiteId}/collections", Method.Post, Creds)
            .WithJsonBody(input, JsonConfig.Settings);
        return Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }

    [Action("Delete collection", Description = "Delete a specific collection")]
    public Task DeleteCollection([ActionParameter] CollectionRequest collectionRequest)
    {
        var request = new WebflowRequest($"collections/{collectionRequest.CollectionId}", Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }
}