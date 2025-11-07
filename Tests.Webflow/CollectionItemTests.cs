using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class CollectionItemTests : TestBase
{
    [TestMethod]
    public async Task DownloadCollectionItem_ReturnsCollectionItem()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new CollectionItemRequest
        {
            CollectionId = "68f8b337cbd1cac54f5b9d9c",
            CollectionItemId = "68f8b337cbd1cac54f5b9dee",
            FileFormat = "original"
        };
        var site = new SiteRequest { };
        var actions = new CollectionItemActions(context, FileManagementClient);

        // Act
        var result = await actions.DownloadCollectionItem(site, request);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchCollectionItems_WithoutFilters_ReturnsCollectionItems()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var actions = new CollectionItemActions(context, FileManagementClient);
        var request = new SearchCollectionItemsRequest { };
        var site = new SiteRequest { };
        var dateFilter = new DateFilter { };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };

        // Act
        var result = await actions.SearchCollectionItems(site, collection, dateFilter, request);

        // Assert
        Assert.IsNotNull(result);
        PrintJsonResult(result);
    }

    [TestMethod]
    public async Task SearchCollectionItems_WithFilters_ReturnsCollectionItems()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var actions = new CollectionItemActions(context, FileManagementClient);
        var request = new SearchCollectionItemsRequest
        {
            LastPublishedAfter = new DateTime(2025, 11, 03, 10, 0, 0, DateTimeKind.Utc),
            CmsLocaleId = "69007d6cf09bd27cf732e15a"
        };
        var site = new SiteRequest { };
        var dateFilter = new DateFilter { };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };

        // Act
        var result = await actions.SearchCollectionItems(site, collection, dateFilter, request);

        // Assert
        Assert.IsNotNull(result);
        PrintJsonResult(result);
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_WithoutPublishing_IsSuccess()
    {
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new UpdateCollectionItemRequest
        {
            File = new FileReference { Name = "12345.xlf" },
            Publish = false,
        };
        var site = new SiteRequest { };
        var actions = new CollectionItemActions(context, FileManagementClient);

        // Act
        await actions.UploadCollectionItem(site, request);
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_WithPublishing_IsSuccess()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var request = new UpdateCollectionItemRequest
            {
                CollectionId = "68f88700e2a4dba6d693cc90",
                CollectionItemId = "68f88700e2a4dba6d693ccc4",
                Publish = true,
                File = new FileReference { Name = "test_en.html" }
            };
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var actions = new CollectionItemActions(context, FileManagementClient);

            // Act
            await actions.UploadCollectionItem(site, request);
        }
    }
}
