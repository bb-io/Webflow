using Apps.Webflow.Models;

namespace Tests.Webflow;

[TestClass]
public class CollectionItemTests :TestBase
{
    [TestMethod]
    public async Task GetCollectionItemContent_ReturnsExpectedItems()
    {
        // Arrange
        var request = new Apps.Webflow.Models.Request.CollectionItem.CollectionItemRequest
        {
            SiteId = "6773fdfb5a841e3420ebc404",
            CollectionId = "68f0bd047d4742cba6c0b30c",
            CollectionItemId= "68f0bd3f34d5e43fc8d1675c"
        };
        var actions = new Apps.Webflow.Actions.CollectionItemActions(InvocationContext, FileManager);
        // Act
        var result = await actions.GetCollectionItemContent(request);
        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_IsSuccess()
    {
        // Arrange
        var request = new Apps.Webflow.Models.Request.Collection.UpdateCollectionItemRequest
        {
            //SiteId = "6773fdfb5a841e3420ebc404",
            //CollectionId = "68f0bd047d4742cba6c0b30c",
            //CollectionItemId = "68f0bd3f34d5e43fc8d1675c"
        };
        var file = new FileModel
        {
            File = new Blackbird.Applications.Sdk.Common.Files.FileReference { Name= "68f0bd3f34d5e43fc8d1675c.html" }
        };
        var actions = new Apps.Webflow.Actions.CollectionItemActions(InvocationContext, FileManager);
        // Act
         await actions.UpdateCollectionItemContent(request, file);
        // Assert
        Assert.IsTrue(true);
    }
}
