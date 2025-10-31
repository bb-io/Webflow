using Apps.Webflow.Constants;
using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Request.Pages;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class DataSources : TestBase
{
    [TestMethod]
    public async Task SiteDataHandler_EmptySearchString_ReturnsSites()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var handler = new SiteDataSourceHandler(context);

            // Act
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            // Assert
            Assert.IsNotNull(data, "Handler returned null.");
            Assert.AreNotEqual(0, data.Count(), "No sites were returned.");

            foreach (var item in data)
                Console.WriteLine($"ID: {item.Value}, Name: {item.DisplayName}");
        }
    }

    [TestMethod]
    public async Task PageDataSourceHandler_ReturnsPages()
    {
        foreach (var context in InvocationContext)
        {
            var input = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };

            //Arange
            var handler = new PageDataSourceHandler(context, input);

            // Act
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            // Assert
            Assert.IsNotNull(data, "Handler returned null.");

            foreach (var item in data)
                Console.WriteLine($"Page ID: {item.Value}, Display Name: {item.DisplayName}");
        }
    }

    [TestMethod]
    public async Task ComponentDataSourceHandler_ReturnsComponents()
    {
        foreach (var context in InvocationContext)
        {
            var request = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };

            // Arrange
            var handler = new ComponentDataSourceHandler(context, request);

            // Act
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            // Assert
            Assert.IsNotNull(data, "Handler returned null.");

            foreach (var item in data)
                Console.WriteLine($"Component ID: {item.Value}, Display Name: {item.DisplayName}");
        }
    }

    [TestMethod]
    public async Task SiteLocaleDataSourceHandler_SearchString_FiltersLocales()
    {
        //Arange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var siteId = "68f8b336cbd1cac54f5b9d2c";
        var request = new SiteRequest { SiteId = siteId };
        // Act
        var handler = new SiteLocaleDataSourceHandler(context, request);

        // Assert
        var data = await handler.GetDataAsync(
            new DataSourceContext { SearchString = "" },
            CancellationToken.None
        );

        foreach (var locale in data)
            Console.WriteLine($"Display name: {locale.DisplayName}, Locale ID: {locale.Value}");
    }

    [TestMethod]
    public async Task CollectionItemCollectionDataSourceHandler_IsSuccess()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var siteId = "";
        var request = new Apps.Webflow.Models.Request.CollectionItem.CollectionItemRequest { };
        var site = new SiteRequest { };

        // Act
        var handler = new CollectionItemCollectionDataSourceHandler(context, site);

        // Assert
        var data = await handler.GetDataAsync(
            new DataSourceContext { SearchString = "" },
            CancellationToken.None
        );

        foreach (var locale in data)
            Console.WriteLine($"Display name: {locale.Key}, Locale ID: {locale.Value}");
    }

    [TestMethod]
    public async Task CollectionItemDataSourceHandler_IsSuccess()
    {
        //Arange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new Apps.Webflow.Models.Request.CollectionItem.CollectionItemRequest
        {
            CollectionId = "68f8b337cbd1cac54f5b9d9c"
        };

        // Act
        var handler = new CollectionItemDataSourceHandler(context, request);

        // Assert
        var data = await handler.GetDataAsync(
            new DataSourceContext { SearchString = "" },
            CancellationToken.None
        );

        foreach (var locale in data)
            Console.WriteLine($"Display name: {locale.Key}, Locale ID: {locale.Value}");
    }

    [TestMethod]
    public async Task CollectionItemLocaleDataSourceHandler_IsSuccess()
    {
        //Arange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };

        // Act
        var handler = new UpdateCollectionItemLocaleDataSourceHandler(context, site);

        // Assert
        var data = await handler.GetDataAsync(
            new DataSourceContext { SearchString = "" },
            CancellationToken.None
        );

        foreach (var locale in data)
            Console.WriteLine($"Display name: {locale.Key}, Locale ID: {locale.Value}");
    }

    [TestMethod]
    public async Task CustomDomainDataSourceHandler_WithSiteId_ReturnsCustomDomains()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var input = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var dataContext = new DataSourceContext { SearchString = "" };
            var handler = new CustomDomainDataSourceHandler(context, input);

            // Act
            var data = await handler.GetDataAsync(dataContext, CancellationToken.None);

            // Assert
            Assert.IsNotNull(data, "Handler returned null.");

            foreach (var item in data)
            {
                Console.WriteLine($"ID: {item.Value}, Name: {item.DisplayName}");
            }
        }
    }

    [TestMethod]
    public async Task CustomDomainDataSourceHandler_WithoutSiteId_ReturnsCustomDomains()
    {
        foreach (var context in InvocationContext)
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
    }
}
