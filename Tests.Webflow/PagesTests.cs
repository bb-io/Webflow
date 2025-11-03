using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Pages;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class PagesTests : TestBase
{
    [TestMethod]
    public async Task SearchPages_ReturnsExpectedPages()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var request = new SearchPagesRequest { };
            var dates = new DateFilter { };

            var actions = new PagesActions(context, FileManagementClient);

            // Act
            var result = await actions.SearchPages(site, request, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task GetPageAsHtml_ReturnsFileReference()
    {

        foreach (var context in InvocationContext)
        {
            // Arrange
            var site = new SiteRequest { SiteId = "6773fdfb5a841e3420ebc404" };
            var input = new DownloadPageRequest { PageId = "6773fdfc5a841e3420ebc46d" };

            var actions = new PagesActions(context, FileManagementClient);

            // Act
            var result = await actions.GetPageAsHtml(site, input);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result, "Result should not be null.");
        }
    }

    [TestMethod]
    public async Task UploadPageFromHtml_SuccessOperation()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var fileReference = new FileReference
            {
                Name = "page_6773fdfc5a841e3420ebc46d.html",
                ContentType = "text/html",
            };

            var site = new SiteRequest { };

            var input = new UpdatePageRequest
            {
                LocaleId = "67765e8a8235a4578faed52a",
                File = fileReference
            };

            var action = new PagesActions(context, FileManagementClient);

            //Act 
            var result = await action.UpdatePageContentAsHtml(site, input);

            //Assert
            Assert.IsTrue(result.Success, "Result should not be null.");
        }
    }
}
