using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Components;

namespace Tests.Webflow;

[TestClass]
public class ComponentsTests : TestBase
{
    // https://webflow.com/dashboard/sites/blackbird-io-blog-news-2d97205311f5fdf6/general
    private const string SampleSiteId = "661d37f0bdd59efaf8124722";
    private const string SampleComponentId = "ece52c30-84e9-6ccc-7181-eb186cf93c46"; // Footer
    private const string SampleTargetLocaleId = "667e7f0ca4540c2dc643af76"; // German

    [TestMethod]
    public async Task SearchComponents_WithoutFilters_ReturnsComponents()
    {
        // Arrange
        var request = new SearchComponentsRequest { };
        var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var actions = new ComponentsActions(InvocationContext, FileManager);

        // Act
        var result = await actions.SearchComponents(site, request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchComponents_WithFilters_ReturnsComponents()
    {
        // Arrange
        var request = new SearchComponentsRequest { NameContains = "ter" };
        var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
        var actions = new ComponentsActions(InvocationContext, FileManager);

        // Act
        var result = await actions.SearchComponents(site, request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetComponentAsHtml_ReturnsFileReference()
    {
        // Arrange
        var input = new GetComponentContentRequest
        {
            SiteId = SampleSiteId,
            ComponentId = SampleComponentId
        };

        var actions = new ComponentsActions(InvocationContext, FileManager);

        // Act
        var result = await actions.GetComponentAsHtml(input);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual("text/html", result.ContentType);
    }

    [TestMethod]
    public async Task UpdateComponentFromHtml_SuccessOperation()
    {
        // Arrange
        var fileReference = await FileManager.UploadTestFileAsync("footer_component_ece52c30-84e9-6ccc-7181-eb186cf93c46.html");

        var input = new UpdateComponentContentRequest
        {
            LocaleId = SampleTargetLocaleId,
            File = fileReference
        };

        var action = new ComponentsActions(InvocationContext, FileManager);

        // Act
        var result = await action.UpdateComponentContentAsHtml(input);

        // Assert
        Assert.IsTrue(result.Success, "Result should be successful.");
    }

    // Helpful to test how well the update worked
    [TestMethod]
    public async Task GetComponentLocaleVersion_ReturnsFileReference()
    {
        // Arrange
        var input = new GetComponentContentRequest
        {
            SiteId = SampleSiteId,
            ComponentId = SampleComponentId,
            LocaleId = SampleTargetLocaleId,
        };

        var actions = new ComponentsActions(InvocationContext, FileManager);

        // Act
        var result = await actions.GetComponentAsHtml(input);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual("text/html", result.ContentType);
    }
}
