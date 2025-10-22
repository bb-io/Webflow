using RestSharp;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Site;
using Apps.Webflow.Models.Response.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.Actions;

[ActionList]
public class SiteActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [Action("Search sites", Description = "Search sites based on search criteria")]
    public async Task<SearchSitesResponse> SearchSites([ActionParameter] SearchSitesRequest input)
    {
        ValidateInputDates(input);

        var request = new RestRequest("sites", Method.Get);
        var result = await Client.ExecuteWithErrorHandling<SiteEntitiesList>(request);

        var filteredResult = ApplySiteFilters(input, result);
        var sites = filteredResult.Select(entity => new GetSiteResponse(entity));
        return new SearchSitesResponse(sites);
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

    private static void ValidateInputDates(SearchSitesRequest input)
    {
        if (!IsCorrectDateRange(input.CreatedBefore, input.CreatedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Created after' date cannot be later than the 'Created before' date");

        if (!IsCorrectDateRange(input.LastPublishedBefore, input.LastPublishedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Last published after' date cannot be later than the 'Last published before' date");
        
        if (!IsCorrectDateRange(input.LastUpdatedBefore, input.LastUpdatedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Last published after' date cannot be later than the 'Last published before' date");
    }

    private static bool IsCorrectDateRange(DateTime? before, DateTime? after)
    {
        if (!before.HasValue || !after.HasValue)
            return true;

        return after <= before;
    }

    private static IEnumerable<SiteEntity> ApplySiteFilters(SearchSitesRequest input, SiteEntitiesList result)
    {
        var sites = result.Sites;

        if (input.CreatedBefore.HasValue)
            sites = sites.Where(x => x.CreatedOn < input.CreatedBefore);

        if (input.CreatedAfter.HasValue)
            sites = sites.Where(x => x.CreatedOn > input.CreatedAfter);

        if (input.LastPublishedBefore.HasValue)
            sites = sites.Where(x => x.LastPublished < input.LastPublishedBefore);

        if (input.LastPublishedAfter.HasValue)
            sites = sites.Where(x => x.LastPublished > input.LastPublishedAfter);

        if (input.LastUpdatedBefore.HasValue)
            sites = sites.Where(x => x.LastUpdated < input.LastUpdatedBefore);

        if (input.LastUpdatedAfter.HasValue)
            sites = sites.Where(x => x.LastUpdated > input.LastUpdatedAfter);

        if (!string.IsNullOrEmpty(input.DisplayNameContains))
            sites = sites.Where(x => x.DisplayName.Contains(input.DisplayNameContains));

        return sites.ToList();
    }
}
