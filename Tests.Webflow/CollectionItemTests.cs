using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Apps.Webflow.Models.Request.Date;
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
            CollectionItemId = "6900ef5e244b95de8a4b7a3c",
            FileFormat = "text/html",
        };
        var locale = new LocaleRequest { Locale = "en" };
        var site = new SiteRequest { };
        var actions = new CollectionItemActions(context, FileManagementClient);

        // Act
        var result = await actions.DownloadCollectionItem(site, request, locale);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UploadCollectionItem_WithoutPublishing_IsSuccess()
    {
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new UpdateCollectionItemRequest
        {
            File = new FileReference { Name = "colitem.html" },
            Publish = false,
        };
        var site = new SiteRequest { };
        var actions = new CollectionItemActions(context, FileManagementClient);
        var locale = new LocaleRequest { Locale = "en" };

        // Act
        await actions.UploadCollectionItem(site, request, locale);
    }

    [TestMethod]
    public async Task UploadCollectionItem_WithPublishing_IsSuccess()
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
            var locale = new LocaleRequest { };

            // Act
            await actions.UploadCollectionItem(site, request, locale);
        }
    }

    [TestMethod]
    public async Task SearchCollectionItems_WithoutFilters_ReturnsCollectionItems()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var actions = new CollectionItemActions(context, FileManagementClient);
        var request = new SearchCollectionItemsRequest { };
        var site = new SiteRequest { };
        var dateFilter = new BasicDateFilter { };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var locale = new LocaleRequest { };

        // Act
        var result = await actions.SearchCollectionItems(site, collection, dateFilter, request, locale);

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
        };
        var site = new SiteRequest { };
        var dateFilter = new BasicDateFilter { };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var locale = new LocaleRequest { };

        // Act
        var result = await actions.SearchCollectionItems(site, collection, dateFilter, request, locale);

        // Assert
        Assert.IsNotNull(result);
        PrintJsonResult(result);
    }

    [TestMethod]
    public async Task PublishCollectionItem_WithoutCmsLocales_IsSuccess()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var action = new CollectionItemActions(context, FileManagementClient);
        var site = new SiteRequest { };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var input = new PublishItemRequest { CollectionItemId = "6900eecfd830d0978a6efd65" };
        var locale = new LocaleRequest { };

        // Act
        await action.PublishCollectionItem(site, collection, input, locale);
    }

    [TestMethod]
    public async Task PublishCollectionItem_WithCmsLocales_IsSuccess()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var action = new CollectionItemActions(context, FileManagementClient);
        var site = new SiteRequest { };
        var input = new PublishItemRequest { CollectionItemId = "6900ef0851221c2ffc49527a" };
        var locale = new LocaleRequest { Locale = "sv-SE" };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };

        // Act
        await action.PublishCollectionItem(site, collection, input, locale);
    }
}
