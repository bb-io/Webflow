using Tests.Webflow.Base;
using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;

namespace Tests.Webflow;

[TestClass]
public class CollectionTests : TestBase
{
	[TestMethod]
	public async Task SearchCollections_WithoutFilters_ReturnsCollections()
	{
		// Arrange
		var context = GetInvocationContext(ConnectionTypes.OAuth2);
		var action = new CollectionActions(context);
		var site = new SiteRequest { };
		var input = new SearchCollectionsRequest { };
		var date = new DateFilter { };

        // Act
        var result = await action.SearchCollections(site, input, date);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchCollections_WithFilters_ReturnsCollections()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var action = new CollectionActions(context);
        var site = new SiteRequest { };
        var input = new SearchCollectionsRequest { SlugContains = "post" };
        var date = new DateFilter { LastUpdatedAfter = new DateTime(2025, 11, 7, 10, 00, 00, DateTimeKind.Utc) };

        // Act
        var result = await action.SearchCollections(site, input, date);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
