using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Apps.Webflow.Models.Request.Date;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class CollectionItemTests : TestBaseWithContext
{
    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task DownloadCollectionItem_ReturnsCollectionItem(InvocationContext context)
    {
        // Arrange
        var request = new DownloadCollectionItemRequest
        {
            CollectionItemId = "6900ef5e244b95de8a4b7a3c",
            FileFormat = ContentFormats.InteroperableHtml,
        };
        var locale = new LocaleIdentifier { Locale = "en" };
        var site = new SiteIdentifier { };
        var actions = new CollectionItemActions(context, FileManagementClient);
        var collection = new CollectionIdentifier { CollectionId = "68f8b337cbd1cac54f5b9d9c" };

        // Act
        var result = await actions.DownloadCollectionItem(site, request, collection, locale);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task UploadCollectionItem_WithoutPublishing_IsSuccess(InvocationContext context)
    {
        var request = new UpdateCollectionItemRequest
        {
            File = new FileReference { Name = "colitem.html" },
            Publish = false,
        };
        var site = new SiteIdentifier { };
        var actions = new CollectionItemActions(context, FileManagementClient);
        var locale = new LocaleIdentifier { Locale = "en" };

        // Act
        await actions.UploadCollectionItem(site, request, locale);
    }

    [TestMethod, ContextDataSource]
    public async Task UploadCollectionItem_WithPublishing_IsSuccess(InvocationContext context)
    {
        // Arrange
        var request = new UpdateCollectionItemRequest
        {
            Publish = true,
            File = new FileReference { Name = "colitem.json" }
        };
        var site = new SiteIdentifier { };
        var actions = new CollectionItemActions(context, FileManagementClient);
        var locale = new LocaleIdentifier { };

        // Act
        await actions.UploadCollectionItem(site, request, locale);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task SearchCollectionItems_WithoutFilters_ReturnsCollectionItems(InvocationContext context)
    {
        // Arrange
        var actions = new CollectionItemActions(context, FileManagementClient);
        var request = new SearchCollectionItemsRequest { };
        var site = new SiteIdentifier { };
        var dateFilter = new BasicDateFilter { };
        var collection = new CollectionIdentifier { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var locale = new LocaleIdentifier { /*Locale = "sv-SE"*/ };

        // Act
        var result = await actions.SearchCollectionItems(site, collection, dateFilter, request, locale);

        // Assert
        Assert.IsNotNull(result);
        PrintResult(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchCollectionItems_WithFilters_ReturnsCollectionItems(InvocationContext context)
    {
        // Arrange
        var actions = new CollectionItemActions(context, FileManagementClient);
        var request = new SearchCollectionItemsRequest
        {
            LastPublishedAfter = new DateTime(2025, 11, 03, 10, 0, 0, DateTimeKind.Utc),
        };
        var site = new SiteIdentifier { };
        var dateFilter = new BasicDateFilter { };
        var collection = new CollectionIdentifier { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var locale = new LocaleIdentifier { };

        // Act
        var result = await actions.SearchCollectionItems(site, collection, dateFilter, request, locale);

        // Assert
        Assert.IsNotNull(result);
        PrintResult(result);
    }

    [TestMethod, ContextDataSource]
    public async Task PublishCollectionItem_WithoutCmsLocales_IsSuccess(InvocationContext context)
    {
        // Arrange
        var action = new CollectionItemActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var collection = new CollectionIdentifier { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var input = new PublishItemRequest { CollectionItemId = "6900eecfd830d0978a6efd65" };
        var locale = new LocaleIdentifier { };

        // Act
        await action.PublishCollectionItem(site, collection, input, locale);
    }

    [TestMethod, ContextDataSource]
    public async Task PublishCollectionItem_WithCmsLocales_IsSuccess(InvocationContext context)
    {
        // Arrange
        var action = new CollectionItemActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var input = new PublishItemRequest { CollectionItemId = "6900ef0851221c2ffc49527a" };
        var locale = new LocaleIdentifier { Locale = "sv-SE" };
        var collection = new CollectionIdentifier { CollectionId = "68f8b337cbd1cac54f5b9d9c" };

        // Act
        await action.PublishCollectionItem(site, collection, input, locale);
    }
}
