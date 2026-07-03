using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities.Page;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Response.Pages;
using Apps.Webflow.Polling.Models;
using Apps.Webflow.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Webflow.Polling;

[PollingEventList]
public class PagePollingList(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    [PollingEvent("On page updated", "Triggers when page update was made")]
    public async Task<PollingEventResponse<PageMemory, SearchPagesResponse>> OnPageUpdated(
        PollingEventRequest<PageMemory> request,
        [PollingEventParameter] SiteIdentifier site,
        [PollingEventParameter] PagePollingRequest input)
    {
        var currentTime = DateTime.UtcNow;
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = currentTime, Triggered = false },
                Result = new SearchPagesResponse([])
            };
        }

        var pagesRequest = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/pages");
        var pagesResponse = await Client.ExecuteWithErrorHandling<SearchPagesResponse>(pagesRequest);

        var lastPollingTime = request.Memory.LastPollingTime ?? DateTime.MinValue;

        IEnumerable<PageEntity> updatedPages = pagesResponse.Pages
            .Where(p => p.LastUpdated.HasValue && p.LastUpdated.Value > lastPollingTime);

        updatedPages = FilterHelper.ApplyDoesNotContainFilter(updatedPages, input.NameDoesNotContain, p => p.Title);
        updatedPages = FilterHelper.ApplyContainsFilter(updatedPages, input.NameContains, p => p.Title);
        updatedPages = FilterHelper.ApplyContainsFilter(updatedPages, input.SlugContains, p => p.Slug);
        updatedPages = FilterHelper.ApplyContainsFilter(updatedPages, input.PublishedPathContains, p => p.PublishedPath);

        var pagesList = updatedPages.ToList();
        bool triggered = pagesList.Count != 0;

        return new PollingEventResponse<PageMemory, SearchPagesResponse>
        {
            FlyBird = triggered,
            Memory = new PageMemory { LastPollingTime = currentTime, Triggered = triggered },
            Result = new SearchPagesResponse(pagesList)
        };
    }
}