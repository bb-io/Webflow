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
                SiteId = "YOUR_SITE_ID",
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
                PageId = "YOUR_PAGE_ID"
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
                PageId = "YOUR_PAGE_ID",
                LocaleId = "YOUR_LOCALE_ID",
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
