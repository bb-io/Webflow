using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Response.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteIdentifier site)
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Client.GetSiteId(site.SiteId)))
            throw new PluginMisconfigurationException("You need to specify Site ID first");

        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/collections", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<SearchCollectionsResponse>(request);

        return response.Collections.Select(c => new DataSourceItem(c.Id, c.DisplayName));
    }
}