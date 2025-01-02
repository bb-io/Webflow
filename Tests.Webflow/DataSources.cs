

using Apps.Webflow.DataSourceHandlers;
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

            var input = new GetPageAsHtmlRequest { SiteId= siteId };

            //Arange
            var handler = new PageDataSourceHandler(InvocationContext, input);

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
    }
}
