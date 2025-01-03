using Apps.Webflow.Actions;
using Apps.Webflow.Models.Request.Pages;

namespace Tests.Webflow
{
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
                Offset = "0",
                Limit = "5",
            };

            var actions = new PagesActions(InvocationContext, FileManager);

            // Act
            var result = await actions.SearchPages(request);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
        }


        [TestMethod]
        public async Task GetPageAsHtml_ReturnsFileReference()
        {
            // Arrange
            var input = new GetPageAsHtmlRequest
            {
                PageId = "6773fdfc5a841e3420ebc46a"
            };

            var actions = new PagesActions(InvocationContext, FileManager);

            // Act
            var result = await actions.GetPageAsHtml(input);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
        }

        [TestMethod]
        public async Task UploadPageFromHtml_SuccessOperation()
        {
            // Arrange
            var fileReference = await FileManager.UploadTestFileAsync("test_en.html");

            var input = new UpdatePageContentRequest
            {
                PageId = "6773fdfc5a841e3420ebc46a",
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
}
