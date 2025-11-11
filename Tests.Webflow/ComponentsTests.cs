using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Components;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class ComponentsTests : TestBaseWithContext
{
    [TestMethod, ContextDataSource]
    public async Task SearchComponents_WithoutFilters_ReturnsComponents(InvocationContext context)
    {
        // Arrange
        var request = new SearchComponentsRequest { };
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var actions = new ComponentsActions(context, FileManagementClient);

        // Act
        var result = await actions.SearchComponents(site, request);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task SearchComponents_WithFilters_ReturnsComponents(InvocationContext context)
    {
        // Arrange
        var request = new SearchComponentsRequest { NameContains = "ter" };
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var actions = new ComponentsActions(context, FileManagementClient);

        // Act
        var result = await actions.SearchComponents(site, request);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2Multiple)]
    public async Task DownloadComponent_WithLocale_ReturnsComponent(InvocationContext context)
    {
        // Arrange
        var actions = new ComponentsActions(context, FileManagementClient);
        
        var site = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var input = new DownloadComponentContentRequest 
        { 
            ComponentId = "88a386dd-8f07-0c34-70f0-2d9f87e29718",
            FileFormat = "text/html"
        };
        var locale = new LocaleRequest { Locale = "sv-SE" };

        // Act
        var result = await actions.DownloadComponent(site, input, locale);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task UploadComponent_IsSuccess(InvocationContext context)
    {
        // Arrange
        var site = new SiteRequest { };
        var action = new ComponentsActions(context, FileManagementClient);

        var fileReference = new FileReference { Name = "comp.json" };
        var input = new UpdateComponentContentRequest { File = fileReference };
        var locale = new LocaleRequest { /*Locale = "sv-SE"*/ };

        // Act
        await action.UploadComponent(site, input, locale);
    }
}
