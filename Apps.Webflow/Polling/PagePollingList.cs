using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
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
    public async Task<PollingEventResponse<PageMemory, ListPagesPollingResponse>> OnPageUpdated(
        PollingEventRequest<PageMemory> request,
        [PollingEventParameter] SiteRequest site,
        [PollingEventParameter] PageUpdatedRequest input)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new(DateTime.UtcNow, false),
                Result = new ListPagesPollingResponse([])
            };
        }

        var pagesRequest = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/pages", Method.Get);
        var pagesResponse = await Client.ExecuteWithErrorHandling<ListPagesPollingResponse>(pagesRequest);

        var lastPollingTime = request.Memory.LastPollingTime ?? DateTime.MinValue;

        IEnumerable<PagePollingEntity> updatedPages = pagesResponse.Pages
            .Where(p => p.LastUpdated.HasValue && p.LastUpdated.Value > lastPollingTime);

        updatedPages = FilterHelper.ApplyDoesNotContainFilter(updatedPages, input.NameDoesNotContain, p => p.Title);
        updatedPages = FilterHelper.ApplyContainsFilter(updatedPages, input.NameContains, p => p.Title);

        bool triggered = updatedPages.Any();

        return new PollingEventResponse<PageMemory, ListPagesPollingResponse>
        {
            FlyBird = triggered,
            Memory = new PageMemory(DateTime.UtcNow, triggered),
            Result = new ListPagesPollingResponse(updatedPages)
        };
    }
}