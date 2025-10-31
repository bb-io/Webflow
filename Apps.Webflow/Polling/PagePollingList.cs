using Apps.Webflow.Invocables;
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
        [PollingEventParameter] PageUpdatedRequest input)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastPollingTime = DateTime.UtcNow,
                    Triggered = false,
                },
                Result = new ListPagesPollingResponse()
            };
        }

        var pagesRequest = new RestRequest($"sites/{input.SiteId}/pages", Method.Get);
        var pagesResponse = await Client.ExecuteWithErrorHandling<ListPagesPollingResponse>(pagesRequest);

        var lastPollingTime = request.Memory.LastPollingTime ?? DateTime.MinValue;
        var updatedPages = pagesResponse.Pages
            .Where(p => p.LastUpdated.HasValue && p.LastUpdated.Value > lastPollingTime)
            .Where(p => string.IsNullOrEmpty(input.NameContains) ||
                        (p.Title != null && p.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        bool triggered = updatedPages.Any();

        var newMemory = new PageMemory
        {
            LastPollingTime = DateTime.UtcNow,
            Triggered = triggered
        };

        return new PollingEventResponse<PageMemory, ListPagesPollingResponse>
        {
            FlyBird = triggered,
            Memory = newMemory,
            Result = new ListPagesPollingResponse
            {
                Pages = updatedPages
            }
        };
    }
}