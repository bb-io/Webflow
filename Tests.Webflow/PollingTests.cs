using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Response.Pages;
using Apps.Webflow.Polling;
using Apps.Webflow.Polling.Models;
using Apps.Webflow.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class PollingTests : TestBase
{
    [TestMethod, ContextDataSource]
    public async Task OnPageUpdated_WithoutNameFilter_ReturnsUpdatedPages(InvocationContext context)
    {
        // Arrange
        var lastPollingTime = DateTime.UtcNow.AddHours(-1);
        var polling = new PagePollingList(context);

        var request = new PollingEventRequest<PageMemory>
        {
            Memory = new PageMemory(lastPollingTime, false)
        };

        var site = new SiteRequest { };
        var input = new PagePollingRequest { };

        // Act
        var response = await polling.OnPageUpdated(request, site, input);

        //Assert
        Assert.IsNotNull(response.Result);
        PrintPollingResult(response);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2, ConnectionTypes.OAuth2Multiple)]
    public async Task OnPageUpdated_WithNameFilter_ReturnsUpdatedPages(InvocationContext context)
    {
        // Arrange
        var lastPollingTime = DateTime.UtcNow.AddHours(-1);
        var polling = new PagePollingList(context);

        var request = new PollingEventRequest<PageMemory>
        {
            Memory = new PageMemory(lastPollingTime, false)
        };

        var input = new PagePollingRequest { NameDoesNotContain = "40" };
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };

        // Act
        var response = await polling.OnPageUpdated(request, site, input);

        //Assert
        Assert.IsNotNull(response.Result);
        PrintPollingResult(response);
    }

    private static void PrintPollingResult(PollingEventResponse<PageMemory, SearchPagesResponse> response)
    {
        foreach (var page in response.Result!.Pages)
            Console.WriteLine($"Page ID: {page.Id}, Title: {page.Title}, Last Updated: {page.LastUpdated}");
    }
}
