using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Site;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Actions;

[ActionList]
public class SiteActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Action("List sites", Description = "Lists all sites")]
    public async Task<ListSitesResponse> ListSites()
    {
        var request = new RestRequest("sites", Method.Get);
        return await Client.ExecuteWithErrorHandling<ListSitesResponse>(request);
    }

    [Action("Get site", Description = "Get details of a site")]
    public async Task<SiteEntity> GetSite([ActionParameter] SiteRequest input)
    {
        var request = new RestRequest($"sites/{input.SiteId}", Method.Get);
        return await Client.ExecuteWithErrorHandling<SiteEntity>(request);
    }

    [Action("Publish site", Description = "Publishes a site to one or more more domains")]
    public async Task<CustomDomainsResponse> PublishSite(
        [ActionParameter] SiteRequest siteInput,
        [ActionParameter] PublishSiteRequest publishInput)
    {
        var request = new RestRequest($"sites/{siteInput.SiteId}/publish", Method.Post);

        if (publishInput.CustomDomains != null)
            request.AddJsonBody(new { customDomains = publishInput.CustomDomains });
        else
            request.AddJsonBody(new { publishToWebflowSubdomain = true });

        return await Client.ExecuteWithErrorHandling<CustomDomainsResponse>(request);
    }
}
