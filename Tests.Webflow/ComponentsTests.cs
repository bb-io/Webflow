using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Components;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class ComponentsTests : TestBase
{
    [TestMethod]
    public async Task SearchComponents_WithoutFilters_ReturnsComponents()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var request = new SearchComponentsRequest { };
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var actions = new ComponentsActions(context, FileManagementClient);

            // Act
            var result = await actions.SearchComponents(site, request);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task SearchComponents_WithFilters_ReturnsComponents()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var request = new SearchComponentsRequest { NameContains = "ter" };
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var actions = new ComponentsActions(context, FileManagementClient);

            // Act
            var result = await actions.SearchComponents(site, request);

            // Assert
            PrintJsonResult(result);
            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    public async Task DownloadComponent_WithLocale_ReturnsComponent()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var actions = new ComponentsActions(context, FileManagementClient);
        
        var site = new SiteRequest { };
        var input = new DownloadComponentContentRequest 
        { 
            ComponentId = "88a386dd-8f07-0c34-70f0-2d9f87e29718",
            FileFormat = "text/html"
        };
        var locale = new LocaleRequest { Locale = "sv-SE" };

        // Act
        var result = await actions.DownloadComponent(site, input, locale);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UploadComponent_IsSuccess()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var site = new SiteRequest { };
        var action = new ComponentsActions(context, FileManagementClient);

        var fileReference = new FileReference { Name = "comp.json" };
        var input = new UpdateComponentContentRequest { File = fileReference };
        var locale = new LocaleRequest { /*Locale = "sv-SE"*/ };

        // Act
        await action.UploadComponent(site, input, locale);
    }
}
