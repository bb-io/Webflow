using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Pages;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class PagesTests : TestBase
{
    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2, ConnectionTypes.OAuth2Multiple)]
    public async Task SearchPages_WithoutFilters_ReturnsPages(InvocationContext context)
    {
        // Arrange
        var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var request = new SearchPagesRequest { };
        var dates = new BasicDateFilter { };

        var actions = new PagesActions(context, FileManagementClient);

        // Act
        var result = await actions.SearchPages(site, request, dates);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task DownloadPage_ReturnsFileReference(InvocationContext context)
    {
        // Arrange
        var site = new SiteRequest { };
        var input = new DownloadPageRequest 
        { 
            FileFormat = "original",
            PageId = "68f8b337cbd1cac54f5b9d80"
        };
        var locale = new LocaleRequest { Locale = "sv-SE" };

        var actions = new PagesActions(context, FileManagementClient);

        // Act
        var result = await actions.DownloadPage(site, input, locale);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task UploadPage_IsSuccess(InvocationContext context)
    {
        // Arrange
        var fileReference = new FileReference { Name = "page.html" };
        var site = new SiteRequest { };

        var input = new UpdatePageContentRequest { File = fileReference };
        var action = new PagesActions(context, FileManagementClient);
        var locale = new LocaleRequest { Locale = "sv-SE" };

        //Act 
        await action.UploadPage(site, input, locale);
    }
}
