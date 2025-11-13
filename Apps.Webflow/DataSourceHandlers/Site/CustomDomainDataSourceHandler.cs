using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Apps.Webflow.Models.Identifiers;

namespace Apps.Webflow.DataSourceHandlers.Site;

public class CustomDomainDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteIdentifier site)
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Client.GetSiteId(site.SiteId)))
            throw new PluginMisconfigurationException("Site ID cannot be null or empty");

        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<CustomDomainsResponse>(request);
        return result.CustomDomains.Select(x => new DataSourceItem(x.Id, x.Url));
    }
}
