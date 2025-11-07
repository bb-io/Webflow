using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Site;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class SiteTests : TestBase
{
    [TestMethod]
    public async Task SearchSites_NoFilters_ReturnsSites()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new SiteActions(context);
            var input = new SearchSitesRequest { };
            var dates = new BasicDateFilter { };

            // Act
            var result = await action.SearchSites(input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchSites_CreatedDateFilter_ReturnsSites()
    {
        foreach (var context in InvocationContext)
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
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchSites_LastPublishedFilter_ReturnsSites()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new SiteActions(context);
            var input = new SearchSitesRequest
            {
                LastPublishedAfter = new DateTime(2025, 10, 22, 10, 0, 0, DateTimeKind.Utc),
                LastPublishedBefore = new DateTime(2025, 10, 22, 11, 0, 0, DateTimeKind.Utc)
            };
            var dates = new BasicDateFilter { };

            // Act
            var result = await action.SearchSites(input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchSites_LastUpdatedFilter_ReturnsSites()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new SiteActions(context);
            var input = new SearchSitesRequest { };
            var dates = new BasicDateFilter
            {
                LastUpdatedAfter = new DateTime(2025, 10, 22, 7, 0, 0, DateTimeKind.Utc),
                LastUpdatedBefore = new DateTime(2025, 10, 22, 8, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var result = await action.SearchSites(input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchSites_DisplayNameFilter_ReturnsSites()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new SiteActions(context);
            var input = new SearchSitesRequest { DisplayNameContains = "Exceptional" };
            var dates = new BasicDateFilter { };

            // Act
            var result = await action.SearchSites(input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
	public async Task GetSite_ReturnsSite()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new SiteActions(context);
            var input = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };

            // Act
            var result = await action.GetSite(input);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

	[TestMethod]
	public async Task PublishSite_WithoutCustomDomains_ReturnsCustomDomains()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new SiteActions(context);
            var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
            var input = new PublishSiteRequest();

            // Act
            var result = await action.PublishSite(site, input);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }
}
