using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem;

public class BaseCollectionItemDataSourceHandler : WebflowInvocable, IAsyncDataSourceHandler
{
    private readonly string _collectionId;
    private readonly string _localeId;

    public BaseCollectionItemDataSourceHandler(InvocationContext invocationContext, string collectionId, string localeId) : base(invocationContext)
    {
        _collectionId = collectionId;
        _localeId = localeId;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_collectionId))
            throw new("You need to specify Collection ID first");

        var endpoint = $"collections/{_collectionId}/items";

        endpoint = string.IsNullOrEmpty(_localeId)
            ? endpoint
            : endpoint.SetQueryParameter("cmsLocaleIds", _localeId);
        var request = new RestRequest(endpoint, Method.Get);
        var response = await Client.Paginate<CollectionItemEntity>(request);

        return response
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.LastUpdated)
            .Take(50)
            .ToDictionary(x => x.Id, x => x.Name);
    }
}