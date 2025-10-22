using Apps.Webflow.Actions;

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
}
