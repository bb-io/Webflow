using Newtonsoft.Json;
using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request.Pages;

namespace Tests.Webflow;

[TestClass]
public class PagesTests : TestBase
{
    [TestMethod]
    public async Task SearchPages_ReturnsExpectedPages()
    {
        // Arrange

        var request = new SearchPagesRequest
        {
            SiteId = "6773fdfb5a841e3420ebc404",
            Draft = true
        };

        var actions = new PagesActions(InvocationContext, FileManager);

        // Act
        var result = await actions.SearchPages(request);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        Console.WriteLine(json);
        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetPageAsHtml_ReturnsFileReference()
    {
        // Arrange
        var input = new GetPageAsHtmlRequest
        {
            SiteId = "6773fdfb5a841e3420ebc404",
            PageId = "6773fdfc5a841e3420ebc46d"
        };

        var actions = new PagesActions(InvocationContext, FileManager);

        // Act
        var result = await actions.GetPageAsHtml(input);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result, "Result should not be null.");
    }

    [TestMethod]
    public async Task UploadPageFromHtml_SuccessOperation()
    {
        // Arrange
        var fileReference = await FileManager.UploadTestFileAsync("page_6773fdfc5a841e3420ebc46d.html");

        var input = new UpdatePageContentRequest
        {
            //PageId = "6773fdfc5a841e3420ebc46b",
            LocaleId = "67765e8a8235a4578faed52a",
            File = fileReference
        };

        var action = new PagesActions(InvocationContext, FileManager);

        //Act 
        var result = await action.UpdatePageContentAsHtml(input);

        //Assert
        Assert.IsTrue(result.Success, "Result should not be null.");
    }
}
