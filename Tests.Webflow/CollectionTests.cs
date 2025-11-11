using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.Date;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class CollectionTests : TestBase
{
	[TestMethod, ContextDataSource]
	public async Task SearchCollections_WithoutFilters_ReturnsCollections(InvocationContext context)
	{
		// Arrange
		var action = new CollectionActions(context);
		var site = new SiteRequest { };
		var input = new SearchCollectionsRequest { };
		var date = new BasicDateFilter { };

        // Act
        var result = await action.SearchCollections(site, input, date);

        // Assert
        PrintResult(result);
		Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchCollections_WithFilters_ReturnsCollections(InvocationContext context)
    {
        // Arrange
        var action = new CollectionActions(context);
        var site = new SiteRequest { };
        var input = new SearchCollectionsRequest { SlugContains = "post" };
        var date = new BasicDateFilter { LastUpdatedAfter = new DateTime(2025, 11, 7, 10, 00, 00, DateTimeKind.Utc) };

        // Act
        var result = await action.SearchCollections(site, input, date);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}
