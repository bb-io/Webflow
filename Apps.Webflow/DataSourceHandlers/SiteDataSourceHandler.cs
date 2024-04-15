using Apps.Webflow.Api;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers;

public class SiteDataSourceHandler : WebflowInvocable, IAsyncDataSourceHandler
{
    public SiteDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new WebflowRequest("sites", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ListSitesResponse>(request);

        return response.Sites
            .Where(x => context.SearchString is null ||
                        x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.LastPublished)
            .Take(50)
            .ToDictionary(x => x.Id, x => x.DisplayName);
    }
}