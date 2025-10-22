using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Site;

public class SiteDataSourceHandler(InvocationContext invocationContext) : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("sites", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<SiteEntitiesList>(request);
        return response.Sites.Select(x => new DataSourceItem(x.Id, x.DisplayName)).ToList();
    }
}