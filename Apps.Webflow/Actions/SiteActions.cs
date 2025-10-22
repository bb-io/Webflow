using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Actions;

[ActionList]
public class SiteActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Action("List sites")]
    public async Task<ListSitesResponse> ListSites()
    {
        var request = new RestRequest("sites", Method.Get);
        return await Client.ExecuteWithErrorHandling<ListSitesResponse>(request);
    }
}
