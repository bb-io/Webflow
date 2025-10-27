using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class ContentTests : TestBase
{
    [TestMethod]
    public async Task SearchContent_AllTypesWithoutFilters_ReturnsPageMetadata()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
            var input = new SearchContentRequest 
            { 
                CollectionId = "68f8b337cbd1cac54f5b9d9c",
                ContentTypes = [ContentTypes.Page, ContentTypes.Component, ContentTypes.CollectionItem] 
            };
            var dates = new DateFilter { };

            // Act
            var result = await action.SearchContent(site, input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchContent_PageTypeWithFilters_ReturnsPageMetadata()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest 
            { 
                /*NameContains = "Pay"*/ 
                ContentTypes = [ContentTypes.Page]
            };
            var dates = new DateFilter { CreatedAfter = new DateTime(2019, 10, 1, 10, 0, 0, DateTimeKind.Utc) };

            // Act
            var result = await action.SearchContent(site, input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchContent_PageTypeWithPublishDateFilter_ThrowsMisconfigException()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest 
            { 
                ContentTypes = [ContentTypes.Page],
                LastPublishedAfter = new DateTime(2025, 10, 10) 
                /*NameContains = "Pay"*/ 
            };
            var dates = new DateFilter { };

            // Act
            var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
                async () => await action.SearchContent(site, input, dates)
            );

            // Assert
            StringAssert.Contains(ex.Message, "'Last published' filter is not supported for Pages");
        }
    }
    
    [TestMethod]
    public async Task SearchContent_ComponentTypeWithoutFilters_ReturnsComponentMetadata()
    {

        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest 
            { 
                ContentTypes = [ContentTypes.Component]            
            };
            var dates = new DateFilter { };

            // Act
            var result = await action.SearchContent(site, input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchContent_ComponentTypeWithNameFilter_ReturnsComponentMetadata()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest 
            { 
                ContentTypes = [ContentTypes.Component],
                NameContains = "Navigation" 
            };
            var dates = new DateFilter { };

            // Act
            var result = await action.SearchContent(site, input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchContent_ComponentTypeWithDateFilter_ThrowsMisconfigException()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest 
            {
                ContentTypes = [ContentTypes.Component]            
            };
            var dates = new DateFilter { CreatedAfter = DateTime.UtcNow };

            // Act
            var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
                async () => await action.SearchContent(site, input, dates)
            );

            // Assert
            StringAssert.Contains(ex.Message, "Date filters are not supported for Components");
        }
    }

    [TestMethod]
    public async Task SearchContent_CollectionItemTypeWithoutFilters_ReturnsCollectionItemMetadata()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest 
            { 
                CollectionId = "68f88700e2a4dba6d693cc90",
                ContentTypes = [ContentTypes.CollectionItem]
            };
            var dates = new DateFilter { };

            // Act
            var result = await action.SearchContent(site, input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchContent_CollectionItemTypeWithFilters_ReturnsCollectionItemMetadata()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest
            {
                ContentTypes = [ContentTypes.CollectionItem], 
                LastPublishedAfter = new DateTime(2025, 10, 22, 5, 0, 0, DateTimeKind.Utc),
                CollectionId = "68f88700e2a4dba6d693cc90",
                /* NameContains = "Effective"*/
            };
            var dates = new DateFilter { LastUpdatedAfter = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc) };

            // Act
            var result = await action.SearchContent(site, input, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }
    
    [TestMethod]
    public async Task SearchContent_CollectionItemTypeWithoutCollectionId_ThrowsMisconfigException()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var action = new ContentActions(context, FileManagementClient);
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var input = new SearchContentRequest
            {
                ContentTypes = [ContentTypes.CollectionItem],
                LastPublishedAfter = new DateTime(2025, 10, 22, 5, 0, 0, DateTimeKind.Utc),
                CollectionId = "",
            };
            var dates = new DateFilter { LastUpdatedAfter = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc) };

            // Act
            var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
                async () => await action.SearchContent(site, input, dates)
            );

            // Assert
            StringAssert.Contains(ex.Message, "Please specify collection ID in order to search content items");
        }
    }

    [TestMethod]
    public async Task DownloadContent_PageTypeWithoutLocaleInput_ReturnsDownloadedContent()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var request = new DownloadContentRequest { ContentId = "68f8b337cbd1cac54f5b9d84" };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.Page };

        // Act
        var result = await action.DownloadContent(site, request, contentFilter);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task DownloadContent_ComponentTypeWithoutLocaleInput_ReturnsDownloadedContent()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.SiteToken);
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var request = new DownloadContentRequest { ContentId = "2df3695a-ff87-37fa-7ac7-63d4f4891939" };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.Component };

        // Act
        var result = await action.DownloadContent(site, request, contentFilter);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task DownloadContent_CollectionItemTypeWithoutLocaleInput_ReturnsDownloadedContent()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var request = new DownloadContentRequest 
        { 
            CollectionId = "68f8b337cbd1cac54f5b9d9b", 
            ContentId = "68f8b337cbd1cac54f5b9df9" 
        };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.CollectionItem };

        // Act
        var result = await action.DownloadContent(site, request, contentFilter);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DownloadContent_CollectionItemTypeWithoutCollectionId_ThrowsMisconfigException()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var request = new DownloadContentRequest
        {
            ContentId = "68f8b337cbd1cac54f5b9df9"
        };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.CollectionItem };

        // Act
        var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(async () =>
            await action.DownloadContent(site, request, contentFilter)
        );

        // Assert
        Assert.Contains(ex.Message, "Collection ID is required");
    }
}
