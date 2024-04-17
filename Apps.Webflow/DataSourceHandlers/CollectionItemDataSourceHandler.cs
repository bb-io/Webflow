using Apps.Webflow.Api;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers;

public class CollectionItemDataSourceHandler : WebflowInvocable, IAsyncDataSourceHandler
{
    private CollectionItemRequest Request { get; }

    public CollectionItemDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] CollectionItemRequest request) : base(invocationContext)
    {
        Request = request;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Request.CollectionId))
            throw new("You need to specify Collection ID first");

        var request = new WebflowRequest($"collections/{Request.CollectionId}/items", Method.Get, Creds);
        var response = await Client.Paginate<CollectionItemEntity>(request);

        return response
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Where(x => Request.CmsLocaleId is null || x.CmsLocaleId == Request.CmsLocaleId)
            .OrderByDescending(x => x.LastUpdated)
            .Take(50)
            .ToDictionary(x => x.Id, x => x.Name);
    }
}