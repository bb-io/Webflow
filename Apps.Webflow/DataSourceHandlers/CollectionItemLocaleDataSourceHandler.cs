using Apps.Webflow.Api;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers;

public class CollectionItemLocaleDataSourceHandler : WebflowInvocable, IAsyncDataSourceHandler
{
    private string SiteId { get; }

    public CollectionItemLocaleDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] CollectionItemRequest request) : base(invocationContext)
    {
        SiteId = request.SiteId;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(SiteId))
            throw new("You need to specify Site ID first");

        var request = new WebflowRequest($"sites/{SiteId}", Method.Get, Creds);
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