using Apps.Webflow.Api;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Response.Collection;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class BaseCollectionDataSourceHandler: WebflowInvocable, IAsyncDataSourceHandler
{
    private string SiteId { get; }

    public BaseCollectionDataSourceHandler(InvocationContext invocationContext, string siteId) : base(invocationContext)
    {
        SiteId = siteId;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(SiteId))
            throw new("You need to specify Site ID first");

        var request = new WebflowRequest($"sites/{SiteId}/collections", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ListCollectionsResponse>(request);

        return response.Collections
            .Where(x => context.SearchString is null ||
                        x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.LastUpdated)
            .Take(50)
            .ToDictionary(x => x.Id, x => x.DisplayName);
    }
}