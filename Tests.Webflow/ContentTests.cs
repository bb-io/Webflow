using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
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
		var contentType = new ContentFilter { ContentType = ContentTypes.Page };
		var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
		var input = new SearchContentRequest { };
		var dates = new DateFilter { };

		// Act
		var result = await action.SearchContent(site, contentType, dates, input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}
}
