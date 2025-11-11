using Apps.Webflow.Constants;
using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class DataSources : TestBase
{
    [TestMethod, ContextDataSource]
    public async Task SiteDataHandler_EmptySearchString_ReturnsSites(InvocationContext context)
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
    public async Task SiteLocaleDataSourceHandler_SearchString_FiltersLocales(InvocationContext context)
    {
        //Arange
        var request = new SiteRequest { };

        // Act
        var handler = new SiteLocaleDataSourceHandler(context, request);
        var data = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotNull(data);
    }

    [TestMethod, ContextDataSource]
    public async Task CollectionItemCollectionDataSourceHandler_IsSuccess(InvocationContext context)
    {
        // Arrange
        var site = new SiteRequest { };
        var handler = new CollectionItemCollectionDataSourceHandler(context, site);

        // Act
        var data = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        foreach (var locale in data)
            Console.WriteLine($"Display name: {locale.Key}, Locale ID: {locale.Value}");
    }

    [TestMethod, ContextDataSource]
    public async Task CollectionItemDataSourceHandler_IsSuccess(InvocationContext context)
    {
        // Arrange
        var site = new SiteRequest { };
        var collection = new CollectionRequest { CollectionId = "68f8b337cbd1cac54f5b9d9c" };
        var locale = new LocaleRequest { Locale = "sv-SE" };
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
        var input = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var dataContext = new DataSourceContext { SearchString = "" };
        var handler = new CustomDomainDataSourceHandler(context, input);

        // Act
        var data = await handler.GetDataAsync(dataContext, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(data);
        Assert.IsNotNull(data);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task CustomDomainDataSourceHandler_WithoutSiteId_ReturnsCustomDomains(InvocationContext context)
    {
        // Arrange
        var input = new SiteRequest { SiteId = "" };
        var dataContext = new DataSourceContext { SearchString = "" };
        var handler = new CustomDomainDataSourceHandler(context, input);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<PluginMisconfigurationException>(
            async () => await handler.GetDataAsync(dataContext, CancellationToken.None)
        );
    }

    private static void PrintDataHandlerResult(IEnumerable<DataSourceItem> items)
    {
        foreach (var item in items)
            Console.WriteLine($"ID: {item.Value}, Display name: {item.DisplayName}");
    }
}
