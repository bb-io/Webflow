using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Collection;
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
    public async Task<CollectionEntity> GetCollection(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionRequest collectionRequest)
    {
        var request = new RestRequest($"collections/{collectionRequest.CollectionId}", Method.Get);
        return await Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }

    [Action("Search collections", Description = "Search all collections for a specific site")]
    public async Task<SearchCollectionsResponse> SearchCollections(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchCollectionsRequest input,
        [ActionParameter] BasicDateFilter dateFilter)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/collections", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<SearchCollectionsResponse>(request);

        IEnumerable<CollectionEntity> filtered = FilterHelper.ApplyDateFilters(result.Collections, dateFilter);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.SlugContains, c => c.Slug);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.DisplayNameContains, c => c.DisplayName);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.SingluralNameContains, c => c.SingularName);

        foreach (var item in filtered)
        {
            var collection = await GetCollection(site, new CollectionRequest { CollectionId = item.Id });
            item.Fields = collection.Fields;
        }

        return new(filtered);
    }

    [Action("Create collection", Description = "Create a new collection")]
    public async Task<CollectionEntity> CreateCollection(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CreateCollectionRequest input)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/collections", Method.Post)
            .WithJsonBody(input, JsonConfig.Settings);
        return await Client.ExecuteWithErrorHandling<CollectionEntity>(request);
    }

    [Action("Delete collection", Description = "Delete a specific collection")]
    public async Task DeleteCollection(
        [ActionParameter] SiteRequest site,
        [ActionParameter] CollectionRequest collectionRequest)
    {
        var request = new RestRequest($"collections/{collectionRequest.CollectionId}", Method.Delete);
        await Client.ExecuteWithErrorHandling(request);
    }
}