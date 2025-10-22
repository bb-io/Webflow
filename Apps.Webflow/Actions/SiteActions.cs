using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Actions;

[ActionList]
public class SiteActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Action("List sites", Description = "List all sites")]
    public async Task<ListSitesResponse> ListSites()
    {
        var request = new RestRequest("sites", Method.Get);
        return await Client.ExecuteWithErrorHandling<ListSitesResponse>(request);
    }

    [Action("Get site", Description = "Get the specific site data and its custom domains")]
    public async Task<SiteEntity> GetSite([ActionParameter] SiteRequest input)
    {
        var request = new RestRequest($"sites/{input.SiteId}", Method.Get);
        return await Client.ExecuteWithErrorHandling<SiteEntity>(request);
    }
}
