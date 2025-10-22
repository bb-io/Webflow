using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Pages;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class SiteLocaleDataSourceHandler(InvocationContext invocationContext, [ActionParameter] UpdatePageContentRequest input) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.SiteId))
            throw new PluginMisconfigurationException("Site ID cannot be null or empty");

        var endpoint = $"sites/{input.SiteId}";
        var request = new RestRequest(endpoint, Method.Get);

        var siteResponse = await Client.ExecuteWithErrorHandling<SiteLocales>(request);

        if (siteResponse.Locales == null)
            return Enumerable.Empty<DataSourceItem>();

        var localeItems = siteResponse.Locales.Secondary
            .Append(siteResponse.Locales.Primary)
            .Where(locale => locale != null)
            .Select(locale => new DataSourceItem
            {
                Value = locale.Id,
                DisplayName = locale.DisplayName
            });

        return localeItems;
    }
}
