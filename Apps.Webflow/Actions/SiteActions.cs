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
        var result = await Client.ExecuteWithErrorHandling<SiteEntitiesList>(request);

        var sites = result.Sites.Select(entity => new GetSiteResponse(entity));
        return new ListSitesResponse(sites);
    }

    [Action("Get site", Description = "Get details of a site")]
    public async Task<GetSiteResponse> GetSite([ActionParameter] SiteRequest input)
    {
        var request = new RestRequest($"sites/{input.SiteId}", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<SiteEntity>(request);
        return new GetSiteResponse(result);
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
