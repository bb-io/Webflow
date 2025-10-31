using Apps.Webflow.Polling;
using Apps.Webflow.Polling.Models;
using Apps.Webflow.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class PollingTests : TestBase
{
    [TestMethod]
    public async Task OnPageUpdated_WithoutNameFilter_ReturnsUpdatedPages()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var lastPollingTime = DateTime.UtcNow.AddHours(-1);
            var polling = new PagePollingList(context);

            var request = new PollingEventRequest<PageMemory>
            {
                Memory = new PageMemory
                {
                    LastPollingTime = lastPollingTime,
                    Triggered = false
                }
            };

            var input = new PageUpdatedRequest {
                SiteId = "68f8b336cbd1cac54f5b9d2c"
            };

            // Act
            var response = polling.OnPageUpdated(request, input);

            //Assert
            Assert.IsNotNull(response, "Response should not be null.");
            foreach (var page in response.Result.Result.Pages)
            {
                Console.WriteLine($"Page ID: {page.Id}, Title: {page.Title}, Last Updated: {page.LastUpdated}");
            }
        }
    }

    [TestMethod]
    public async Task OnPageUpdated_WithNameFilter_ReturnsUpdatedPages()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var lastPollingTime = DateTime.UtcNow.AddHours(-1);
            var polling = new PagePollingList(context);

            var request = new PollingEventRequest<PageMemory>
            {
                Memory = new PageMemory
                {
                    LastPollingTime = lastPollingTime,
                    Triggered = false
                }
            };

            var input = new PageUpdatedRequest
            {
                SiteId = "68f8b336cbd1cac54f5b9d2c",
                NameContains = "Abo"
            };

            // Act
            var response = polling.OnPageUpdated(request, input);

            //Assert
            Assert.IsNotNull(response, "Response should not be null.");
            foreach (var page in response.Result.Result.Pages)
            {
                Console.WriteLine($"Page ID: {page.Id}, Title: {page.Title}, Last Updated: {page.LastUpdated}");
            }
        }
    }
}
