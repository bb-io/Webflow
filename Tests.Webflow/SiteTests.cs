using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Site;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class SiteTests : TestBaseWithContext
{
    [TestMethod, ContextDataSource]
    public async Task SearchSites_WithoutFilters_ReturnsSites(InvocationContext context)
    {
        // Arrange
        var action = new SiteActions(context);
        var input = new SearchSitesRequest { };
        var dates = new BasicDateFilter { };

        // Act
        var result = await action.SearchSites(input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchSites_WithFilters_ReturnsSites(InvocationContext context)
    {
        // Arrange
        var action = new SiteActions(context);
        var input = new SearchSitesRequest { };
        var dates = new BasicDateFilter
        {
            CreatedAfter = new DateTime(2025, 10, 22, 7, 0, 0, DateTimeKind.Utc),
            CreatedBefore = new DateTime(2025, 10, 22, 10, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await action.SearchSites(input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
	public async Task GetSite_ReturnsSite(InvocationContext context)
    {
        // Arrange
        var action = new SiteActions(context);
        var input = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };

        // Act
        var result = await action.GetSite(input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

	[TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
	public async Task PublishSite_WithoutCustomDomains_ReturnsCustomDomains(InvocationContext context)
    {
        // Arrange
        var action = new SiteActions(context);
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var input = new PublishSiteRequest();

        // Act
        var result = await action.PublishSite(site, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}
