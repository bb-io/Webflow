using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Date;
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
            var dates = new BasicDateFilter { };

            var actions = new PagesActions(context, FileManagementClient);

            // Act
            var result = await actions.SearchPages(site, request, dates);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task DownloadPage_ReturnsFileReference()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var site = new SiteRequest { };
        var input = new DownloadPageRequest 
        { 
            FileFormat = "text/html",
            PageId = "68f8b337cbd1cac54f5b9d80", 
            LocaleId = "69007d6cf09bd27cf732e155"
        };

        var actions = new PagesActions(context, FileManagementClient);

        // Act
        var result = await actions.DownloadPage(site, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UploadPage_SuccessOperation()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var fileReference = new FileReference { Name = "page.xlf" };
        var site = new SiteRequest { };

        var input = new UpdatePageContentRequest { File = fileReference };
        var action = new PagesActions(context, FileManagementClient);

        //Act 
        await action.UploadPage(site, input);
    }
}
