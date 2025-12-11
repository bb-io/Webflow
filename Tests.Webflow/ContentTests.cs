using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class ContentTests : TestBaseWithContext
{
    [TestMethod, ContextDataSource]
    public async Task SearchContent_AllTypesWithoutFilters_ReturnsPageMetadata(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var input = new SearchContentRequest 
        { 
            CollectionIds = ["68f8b337cbd1cac54f5b9d9c"],
        };
        var dates = new ContentDateFilter { };

        // Act
        var result = await action.SearchContent(site, input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchContent_PageTypeWithFilters_ReturnsPageMetadata(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var input = new SearchContentRequest 
        { 
            /*NameContains = "Pay"*/ 
            ContentTypes = [ContentTypes.Page]
        };
        var dates = new ContentDateFilter { CreatedAfter = new DateTime(2019, 10, 1, 10, 0, 0, DateTimeKind.Utc) };

        // Act
        var result = await action.SearchContent(site, input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchContent_PageTypeWithPublishDateFilter_ThrowsMisconfigException(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var input = new SearchContentRequest 
        { 
            ContentTypes = [ContentTypes.Page],
            LastPublishedAfter = new DateTime(2025, 10, 10) 
            /*NameContains = "Pay"*/ 
        };
        var dates = new ContentDateFilter { };

        // Act
        var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
            async () => await action.SearchContent(site, input, dates)
        );

        // Assert
        Assert.Contains("'Last published' filter is not supported for Pages", ex.Message);
    }
    
    [TestMethod, ContextDataSource]
    public async Task SearchContent_ComponentTypeWithoutFilters_ReturnsComponentMetadata(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var input = new SearchContentRequest 
        { 
            ContentTypes = [ContentTypes.Component]            
        };
        var dates = new ContentDateFilter { };

        // Act
        var result = await action.SearchContent(site, input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task SearchContent_ComponentTypeWithFilters_ReturnsComponentMetadata(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var input = new SearchContentRequest 
        { 
            ContentTypes = [ContentTypes.Component],
            DescriptionContains = "abba",
            NameContains = "news"
        };
        var dates = new ContentDateFilter { };

        // Act
        var result = await action.SearchContent(site, input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchContent_ComponentTypeWithDateFilter_ThrowsMisconfigException(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var input = new SearchContentRequest 
        {
            ContentTypes = [ContentTypes.Component]            
        };
        var dates = new ContentDateFilter { CreatedAfter = DateTime.UtcNow };

        // Act
        var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
            async () => await action.SearchContent(site, input, dates)
        );

        // Assert
        Assert.Contains(ex.Message, "Date filters are not supported for Components");
    }

    [TestMethod, ContextDataSource]
    public async Task SearchContent_CollectionItemTypeWithoutFilters_ReturnsCollectionItemMetadata(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var input = new SearchContentRequest 
        { 
            CollectionIds = ["68f88700e2a4dba6d693cc90"],
            ContentTypes = [ContentTypes.CollectionItem]
        };
        var dates = new ContentDateFilter { };

        // Act
        var result = await action.SearchContent(site, input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchContent_CollectionItemTypeWithFilters_ReturnsCollectionItemMetadata(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var input = new SearchContentRequest
        {
            ContentTypes = [ContentTypes.CollectionItem], 
            LastPublishedBefore = new DateTime(2025, 11, 3, 5, 0, 0, DateTimeKind.Utc),
            CollectionIds = ["68f8b337cbd1cac54f5b9d9c"],
        };
        var dates = new ContentDateFilter { LastUpdatedAfter = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc) };

        // Act
        var result = await action.SearchContent(site, input, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, ContextDataSource]
    public async Task SearchContent_CollectionItemTypeWithoutCollectionId_ThrowsMisconfigException(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var input = new SearchContentRequest
        {
            ContentTypes = [ContentTypes.CollectionItem],
            LastPublishedAfter = new DateTime(2025, 10, 22, 5, 0, 0, DateTimeKind.Utc),
            CollectionIds = [],
        };
        var dates = new ContentDateFilter { LastUpdatedAfter = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc) };

        // Act
        var ex = await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
            async () => await action.SearchContent(site, input, dates)
        );

        // Assert
        Assert.Contains(ex.Message, "Please specify at least one collection ID in order to search content items");
    }

    [TestMethod, ContextDataSource(ConnectionTypes.SiteToken)]
    public async Task DownloadContent_PageTypeWithoutLocaleInput_ReturnsDownloadedContent(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var request = new DownloadContentRequest 
        { 
            ContentId = "68f8b337cbd1cac54f5b9d81", 
            IncludeMetadata = true,
        };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.Page };

        // Act
        var result = await action.DownloadContent(site, request, contentFilter);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, ContextDataSource]
    public async Task DownloadContent_ComponentTypeWithoutLocaleInput_ReturnsDownloadedContent(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var request = new DownloadContentRequest 
        { 
            ContentId = "88a386dd-8f07-0c34-70f0-2d9f87e29718", 
            FileFormat = ContentFormats.OriginalJson
        };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.Component };

        // Act
        var result = await action.DownloadContent(site, request, contentFilter);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task DownloadContent_CollectionItemTypeWithoutLocaleInput_ReturnsDownloadedContent(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var request = new DownloadContentRequest 
        {
            CollectionId = "69039bc9c5d438759be4cc0c",
            ContentId = "69039bc9c5d438759be4cd62",
            FileFormat = ContentFormats.InteroperableHtml,
            IncludeSlug = true,
            Locale = "en"
        };
        var contentFilter = new ContentFilter { ContentType = ContentTypes.CollectionItem };

        // Act
        var result = await action.DownloadContent(site, request, contentFilter);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task DownloadContent_CollectionItemTypeWithoutCollectionId_ThrowsMisconfigException(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
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

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task UploadContent_PageType_IsSuccess(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var request = new UploadContentRequest
        {
            Content = new FileReference { Name = "page.xlf" },
            Locale = "sv-SE"
        };

        // Act
        await action.UploadContent(site, request);
    }
    
    [TestMethod, ContextDataSource]
    public async Task UploadContent_ComponentType_IsSuccess(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var request = new UploadContentRequest
        {
            Content = new FileReference { Name = "comp.html" },
        };

        // Act
        await action.UploadContent(site, request);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task UploadContent_CollectionItemType_IsSuccess(InvocationContext context)
    {
        // Arrange
        var action = new ContentActions(context, FileManagementClient);
        var site = new SiteIdentifier { };
        var request = new UploadContentRequest
        {
            Content = new FileReference { Name = "test.json" },
            Locale = "en"
        };

        // Act
        await action.UploadContent(site, request);
    }
}
