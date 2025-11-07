using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class SiteLocaleDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest site) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Client.GetSiteId(site.SiteId)))
            throw new PluginMisconfigurationException("Site ID cannot be null or empty");

        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}";
        var request = new RestRequest(endpoint, Method.Get);
        var siteResponse = await Client.ExecuteWithErrorHandling<SiteEntity>(request);

        var result = new List<DataSourceItem>();
        var primaryLocale = new DataSourceItem(siteResponse.Locales.Primary.Tag, siteResponse.Locales.Primary.DisplayName);
        var secondaryLocales = siteResponse.Locales.Secondary.Select(l => new DataSourceItem(l.Tag, l.DisplayName));

        result.Add(primaryLocale);
        result.AddRange(secondaryLocales);
        return result;
    }
}
