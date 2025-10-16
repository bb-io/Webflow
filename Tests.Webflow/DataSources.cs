

using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Pages;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Tests.Webflow
{
    [TestClass]
    public class DataSources : TestBase
    {
        [TestMethod]
        public async Task SiteDataHandler_EmptySearchString_ReturnsSites()
        {
            // Arrange
            var handler = new SiteDataSourceHandler(InvocationContext);

            // Act
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            // Assert
            Assert.IsNotNull(data, "Handler returned null.");
            Assert.AreNotEqual(0, data.Count, "No sites were returned.");

            foreach (var item in data)
            {
                Console.WriteLine($"ID: {item.Key}, Name: {item.Value}");
            }
        }

        [TestMethod]
        public async Task PageDataSourceHandler_ReturnsPages()
        {
            var siteId = "6773fdfb5a841e3420ebc404";

            var input = new GetPageAsHtmlRequest { SiteId = siteId };

            //Arange
            var handler = new PageDataSourceHandler(InvocationContext, input);

            // Act
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            // Assert
            Assert.IsNotNull(data, "Handler returned null.");

            foreach (var item in data)
            {
                Console.WriteLine($"Page ID: {item.Value}, Display Name: {item.DisplayName}");

            }
        }

        [TestMethod]
        public async Task SiteLocaleDataSourceHandler_SearchString_FiltersLocales()
        {
            //Arange
            var siteId = "661d37f0bdd59efaf8124722";
            var request = new UpdatePageContentRequest { SiteId = siteId };
            // Act
            var handler = new SiteLocaleDataSourceHandler(InvocationContext, request);

            // Assert
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            foreach (var locale in data)
            {
                Console.WriteLine($"Display name: {locale.DisplayName}, Locale ID: {locale.Value}");
            }
        }

        [TestMethod]
        public async Task CollectionItemCollectionDataSourceHandler_IsSuccess()
        {
            //Arange
            var siteId = "6773fdfb5a841e3420ebc404";
            var request = new Apps.Webflow.Models.Request.CollectionItem.CollectionItemRequest { SiteId = siteId };
            // Act
            var handler = new CollectionItemCollectionDataSourceHandler(InvocationContext, request);

            // Assert
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            foreach (var locale in data)
            {
                Console.WriteLine($"Display name: {locale.Key}, Locale ID: {locale.Value}");
            }
        }
        //6682a16446a95c6b7b7ef118 CollectionItemDataSourceHandler

        [TestMethod]
        public async Task CollectionItemDataSourceHandler_IsSuccess()
        {
            //Arange
            var siteId = "661d37f0bdd59efaf8124722";
            var request = new Apps.Webflow.Models.Request.CollectionItem.CollectionItemRequest { SiteId = siteId,
                CollectionId = "68f0bd047d4742cba6c0b30c"
            }; 
            // Act
            var handler = new CollectionItemDataSourceHandler(InvocationContext, request);

            // Assert
            var data = await handler.GetDataAsync(
                new DataSourceContext { SearchString = "" },
                CancellationToken.None
            );

            foreach (var locale in data)
            {
                Console.WriteLine($"Display name: {locale.Key}, Locale ID: {locale.Value}");
            }
        }
    }
}
