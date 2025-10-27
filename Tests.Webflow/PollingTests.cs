using Apps.Webflow.Polling;
using Apps.Webflow.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class PollingTests : TestBase
{
    [TestMethod]
    public async Task OnPageUpdated_ReturnsUpdatedPages()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var siteId = "YOUR_SITE_ID";
            var lastPollingTime = DateTime.UtcNow.AddHours(-10);
            var polling = new PagePollingList(context);

            var request = new PollingEventRequest<PageMemory>
            {
                Memory = new PageMemory
                {
                    LastPollingTime = lastPollingTime,
                    Triggered = false
                }
            };

            // Act
            var response = polling.OnPageUpdated(request, siteId);

            //Assert
            Assert.IsNotNull(response, "Response should not be null.");
            foreach (var page in response.Result.Result.Pages)
            {
                Console.WriteLine($"Page ID: {page.Id}, Title: {page.Title}, Last Updated: {page.LastUpdated}");
            }
        }
    }
}
