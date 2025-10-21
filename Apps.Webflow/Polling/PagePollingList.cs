using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.Invocables;
using Apps.Webflow.Polling.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Webflow.Polling
{
    [PollingEventList]
    public class PagePollingList : WebflowInvocable
    {
        public PagePollingList(InvocationContext invocationContext) : base(invocationContext) { }

        [PollingEvent("On page update", "Triggered when page update was made")]
        public async Task<PollingEventResponse<PageMemory, ListPagesPollingResponse>> OnPageUpdated(
            PollingEventRequest<PageMemory> request,
            [PollingEventParameter][Display("Site ID")][DataSource(typeof(SiteDataSourceHandler))] string siteId)
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

            var pagesRequest = new RestRequest($"sites/{siteId}/pages", Method.Get);
            var pagesResponse = await Client.ExecuteWithErrorHandling<ListPagesPollingResponse>(pagesRequest);

            var lastPollingTime = request.Memory.LastPollingTime ?? DateTime.MinValue;
            var updatedPages = pagesResponse.Pages
                .Where(p => p.LastUpdated.HasValue && p.LastUpdated.Value > lastPollingTime)
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
}