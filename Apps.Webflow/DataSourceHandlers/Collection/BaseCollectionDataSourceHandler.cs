using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Response.Collection;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class BaseCollectionDataSourceHandler(InvocationContext invocationContext, string siteId) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Client.GetSiteId(siteId)))
            throw new("You need to specify Site ID first");

        var request = new RestRequest($"sites/{Client.GetSiteId(siteId)}/collections", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<SearchCollectionsResponse>(request);

        return response.Collections
            .Where(x => context.SearchString is null ||
                        x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.LastUpdated)
            .Take(50)
            .ToDictionary(x => x.Id, x => x.DisplayName);
    }
}