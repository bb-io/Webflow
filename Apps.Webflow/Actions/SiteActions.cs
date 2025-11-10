using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Site;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList("Sites")]
public class SiteActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Action("Search sites", Description = "Search sites based on search criteria")]
    public async Task<SearchSitesResponse> SearchSites(
        [ActionParameter] SearchSitesRequest input,
        [ActionParameter] BasicDateFilter dateFilter)
    {
        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        var request = new RestRequest("sites", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<SiteEntitiesList>(request);

        var filteredResult = ApplySiteFilters(input, dateFilter, result);
        var sites = filteredResult.Select(entity => new GetSiteResponse(entity));
        return new(sites);
    }

    [Action("Get site", Description = "Get details of a site")]
    public async Task<GetSiteResponse> GetSite([ActionParameter] SiteRequest site)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<SiteEntity>(request);
        return new(result);
    }

    [Action("Publish site", Description = "Publishes a site to one or more more domains")]
    public async Task<CustomDomainsResponse> PublishSite(
        [ActionParameter] SiteRequest site,
        [ActionParameter] PublishSiteRequest publishInput)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/publish", Method.Post);

        if (publishInput.CustomDomains != null)
            request.AddJsonBody(new { customDomains = publishInput.CustomDomains });
        else
            request.AddJsonBody(new { publishToWebflowSubdomain = true });

        return await Client.ExecuteWithErrorHandling<CustomDomainsResponse>(request);
    }

    private static List<SiteEntity> ApplySiteFilters(SearchSitesRequest input, BasicDateFilter date, SiteEntitiesList result)
    {
        IEnumerable<SiteEntity> sites = result.Sites;

        if (date.CreatedBefore.HasValue)
            sites = sites.Where(x => x.CreatedOn < date.CreatedBefore);

        if (date.CreatedAfter.HasValue)
            sites = sites.Where(x => x.CreatedOn > date.CreatedAfter);

        if (input.LastPublishedBefore.HasValue)
            sites = sites.Where(x => x.LastPublished < input.LastPublishedBefore);

        if (input.LastPublishedAfter.HasValue)
            sites = sites.Where(x => x.LastPublished > input.LastPublishedAfter);

        if (date.LastUpdatedBefore.HasValue)
            sites = sites.Where(x => x.LastUpdated < date.LastUpdatedBefore);

        if (date.LastUpdatedAfter.HasValue)
            sites = sites.Where(x => x.LastUpdated > date.LastUpdatedAfter);

        sites = FilterHelper.ApplyContainsFilter(sites, input.DisplayNameContains, s => s.DisplayName);

        return sites.ToList();
    }
}
