using Apps.Webflow.Actions;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class CollectionItemTests : TestBase
{
    [TestMethod]
    public async Task GetCollectionItemContent_ReturnsExpectedItems()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var request = new CollectionItemRequest
            {
                CollectionId = "68f0bd047d4742cba6c0b30c",
                CollectionItemId = "68f0bd3f34d5e43fc8d1675c"
            };
            var site = new SiteRequest { SiteId = "6773fdfb5a841e3420ebc404" };
            var actions = new CollectionItemActions(context, FileManagementClient);
            // Act
            var result = await actions.GetCollectionItemContent(site, request);
            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
        }
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_WithoutPublishing_IsSuccess()
    {
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new UpdateCollectionItemRequest
        {
            File = new FileReference { Name = "12345.xlf" }
        };
        var site = new SiteRequest { };
        var actions = new CollectionItemActions(context, FileManagementClient);

        // Act
        await actions.UpdateCollectionItemContent(site, request);
    }

    [TestMethod]
    public async Task UpdateCollectionItemContent_WithPublishing_IsSuccess()
    {
        foreach (var context in InvocationContext)
        {
            // Arrange
            var request = new UpdateCollectionItemRequest
            {
                CollectionId = "68f88700e2a4dba6d693cc90",
                CollectionItemId = "68f88700e2a4dba6d693ccc4",
                Publish = true,
                File = new FileReference { Name = "test_en.html" }
            };
            var site = new SiteRequest { SiteId = "68f886ffe2a4dba6d693cbe1" };
            var actions = new CollectionItemActions(context, FileManagementClient);

            // Act
            await actions.UpdateCollectionItemContent(site, request);
        }
    }
}
