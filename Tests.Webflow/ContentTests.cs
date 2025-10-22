using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request.Content;

namespace Tests.Webflow;

[TestClass]
public class ContentTests : TestBase
{
    [TestMethod]
    public async Task SearchContent_PageType_ReturnsPageMetadata()
    {
		// Arrange
		var action = new ContentActions(InvocationContext);
		var contentType = new ContentFilter { ContentType = "page" };
		var input = new SearchContentRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };

		// Act
		var result = await action.SearchContent(contentType, input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}
}
