using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class CmsLocaleDataSourceHandler :  WebflowInvocable, IAsyncDataSourceHandler
{
    private string SiteId { get; }

    public CmsLocaleDataSourceHandler(InvocationContext invocationContext, string siteId) : base(invocationContext)
    {
        SiteId = siteId;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(SiteId))
            throw new("You need to specify Site ID first");

        var request = new RestRequest($"sites/{SiteId}", Method.Get);
        var site = await Client.ExecuteWithErrorHandling<SiteEntity>(request);

        if (site.Locales is null)
            return new();
        
        return site.Locales.Secondary
            .Append(site.Locales.Primary)
            .Where(x => context.SearchString is null ||
                        x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.CmsLocaleId, x => x.DisplayName);
    }
}