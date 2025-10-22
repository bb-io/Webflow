using Apps.Webflow.Actions;
using Apps.Webflow.Models;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common.Files;

namespace Tests.Webflow;

[TestClass]
public class CollectionItemTests :TestBase
{
    [TestMethod]
    public async Task GetCollectionItemContent_ReturnsExpectedItems()
    {
        // Arrange
        var request = new CollectionItemRequest
        {
            SiteId = "6773fdfb5a841e3420ebc404",
            CollectionId = "68f0bd047d4742cba6c0b30c",
            CollectionItemId= "68f0bd3f34d5e43fc8d1675c"
        };
        var actions = new CollectionItemActions(InvocationContext, FileManager);
        // Act
        var result = await actions.GetCollectionItemContent(request);
        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_WithoutPublishing_IsSuccess()
    {
        // Arrange
        var request = new UpdateCollectionItemRequest
        {
            SiteId = "68f886ffe2a4dba6d693cbe1",
            CollectionId = "68f88700e2a4dba6d693cc90",
            CollectionItemId = "68f88700e2a4dba6d693ccc4"
        };
        var file = new FileModel
        {
            File = new FileReference { Name = "test_en.html" }
        };
        var actions = new CollectionItemActions(InvocationContext, FileManager);
        
        // Act
        var result = await actions.UpdateCollectionItemContent(request, file);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_WithPublishing_IsSuccess()
    {
        // Arrange
        var request = new UpdateCollectionItemRequest
        {
            SiteId = "68f886ffe2a4dba6d693cbe1",
            CollectionId = "68f88700e2a4dba6d693cc90",
            CollectionItemId = "68f88700e2a4dba6d693ccc4",
            Publish = true
        };
        var file = new FileModel
        {
            File = new FileReference { Name = "test_en.html" }
        };
        var actions = new CollectionItemActions(InvocationContext, FileManager);

        // Act
        var result = await actions.UpdateCollectionItemContent(request, file);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
