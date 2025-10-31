using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Response.Components;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers;

public class ComponentDataSourceHandler(
    InvocationContext invocationContext,
    [ActionParameter] SiteRequest site) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/components";
        var request = new RestRequest(endpoint, Method.Get);

        var response = await Client.ExecuteWithErrorHandling<SearchComponentsResponse>(request);

        var dataSourceItems = response.Components?
            .Select(component => new DataSourceItem
            {
                Value = component.Id,
                DisplayName = component.Name
            })
            .ToList() ?? new List<DataSourceItem>();

        return dataSourceItems;
    }
}
