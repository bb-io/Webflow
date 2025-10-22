using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.DataSourceHandlers.Site;

public class CustomDomainDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest input)
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.SiteId))
            throw new PluginMisconfigurationException("Site ID cannot be null or empty");

        var request = new RestRequest($"sites/{input.SiteId}", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<CustomDomainsResponse>(request);
        return result.CustomDomains.Select(x => new DataSourceItem(x.Id, x.Url));
    }
}
