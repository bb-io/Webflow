using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request;

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
}
