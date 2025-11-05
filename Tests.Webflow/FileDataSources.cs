using Tests.Webflow.Base;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.DataSourceHandlers.Component;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Tests.Webflow;

[TestClass]
public class FileDataSources : TestBase
{
    [TestMethod]
    public async Task ComponentFileDataSourceHandler_GetFolderPathAsync_ReturnsComponentGroups()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var folderContext = new FolderPathDataSourceContext { FileDataItemId = "88a386dd-8f07-0c34-70f0-2d9f87e29718" };
        var handler = new ComponentFileDataSourceHandler(context, request);

        // Act
        var data = await handler.GetFolderPathAsync(folderContext, CancellationToken.None);

        // Assert
        Assert.IsNotNull(data);

        foreach (var item in data)
            Console.WriteLine($"Folder ID: {item.Id}, Display Name: {item.DisplayName}");
    }

    [TestMethod]
    public async Task ComponentFileDataSourceHandler_GetFolderContentAsync_ReturnsComponentIds()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new SiteRequest { SiteId = "68f8b336cbd1cac54f5b9d2c" };
        var folderContext = new FolderContentDataSourceContext { FolderId = "Root" };
        var handler = new ComponentFileDataSourceHandler(context, request);

        // Act
        var data = await handler.GetFolderContentAsync(folderContext, CancellationToken.None);

        // Assert
        Assert.IsNotNull(data);

        foreach (var item in data)
            Console.WriteLine($"Folder ID: {item.Id}, Display Name: {item.DisplayName}, Type: {item.Type}");
    }
}
