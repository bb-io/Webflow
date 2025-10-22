using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Site;

namespace Tests.Webflow;

[TestClass]
public class SiteTests : TestBase
{
    [TestMethod]
    public async Task ListSites_ReturnsSites()
    {
		// Arrange
		var action = new SiteActions(InvocationContext);

		// Act
		var result = await action.ListSites();

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public async Task GetSite_ReturnsSite()
	{
        // Arrange
        var action = new SiteActions(InvocationContext);
		var input = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };

        // Act
		var result = await action.GetSite(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task PublishSite_WithoutCustomDomains_ReturnsCustomDomains()
	{
		// Arrange
		var action = new SiteActions(InvocationContext);
		var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
		var input = new PublishSiteRequest();

		// Act
		var result = await action.PublishSite(site, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
