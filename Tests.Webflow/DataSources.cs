using Apps.Webflow.Constants;
using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Content;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class DataSources : TestBaseWithContext
{
    [TestMethod, ContextDataSource]
    public async Task SiteDataHandler_ReturnsSites(InvocationContext context)
    {
        // Arrange
        var handler = new SiteDataSourceHandler(context);

        // Act
        var data = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotNull(data);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task SiteLocaleDataSourceHandler_ReturnsLocales(InvocationContext context)
    {
        //Arange
        var request = new SiteIdentifier { };

        // Act
        var handler = new SiteLocaleDataSourceHandler(context, request);
        var data = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotNull(data);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2, ConnectionTypes.OAuth2Multiple)]
    public async Task CollectionDataSourceHandler_ReturnsCollections(InvocationContext context)
    {
        // Arrange
        var site = new SiteIdentifier { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var handler = new CollectionDataSourceHandler(context, site);

        // Act
        var data = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotEmpty(data);
    }

    [TestMethod, ContextDataSource]
    public async Task CollectionItemDataSourceHandler_ReturnsCollectionItems(InvocationContext context)
    {
        // Arrange
        var site = new SiteIdentifier { };
        var collection = new CollectionIdentifier { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var locale = new LocaleIdentifier { Locale = "sv-SE" };
        var handler = new CollectionItemDataSourceHandler(context, site, collection, locale);

        // Act
        var data = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotEmpty(data);
    }

    [TestMethod, ContextDataSource]
    public async Task CustomDomainDataSourceHandler_WithSiteId_ReturnsCustomDomains(InvocationContext context)
    {
        // Arrange
        var input = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var dataContext = new DataSourceContext { SearchString = "" };
        var handler = new CustomDomainDataSourceHandler(context, input);

        // Act
        var data = await handler.GetDataAsync(dataContext, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotNull(data);
    }

    [TestMethod, ContextDataSource]
    public async Task CustomDomainDataSourceHandler_WithoutSiteId_ThrowsMisconfigException(InvocationContext context)
    {
        // Arrange
        var input = new SiteIdentifier { SiteId = "" };
        var dataContext = new DataSourceContext { SearchString = "" };
        var handler = new CustomDomainDataSourceHandler(context, input);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
            async () => await handler.GetDataAsync(dataContext, CancellationToken.None)
        );
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2Multiple)]
    public async Task ContentDataHandler_ReturnsContent(InvocationContext context)
    {
        // Arrange
        var input = new SiteIdentifier { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var filter = new ContentFilter { ContentType = ContentTypes.Component };
        var dataContext = new DataSourceContext { SearchString = "" };
        var handler = new ContentDataHandler(context, filter, input, "");

        // Act
        var data = await handler.GetDataAsync(dataContext, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotNull(data);
    }
}
