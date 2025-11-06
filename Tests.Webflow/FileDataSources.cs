using Tests.Webflow.Base;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Request;
using Apps.Webflow.DataSourceHandlers.Component;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Apps.Webflow.DataSourceHandlers.Pages;

namespace Tests.Webflow;

[TestClass]
public class FileDataSources : TestBase
{
    [TestMethod]
    public async Task ComponentFileDataSourceHandler_GetFolderPathAsync_ReturnsComponentGroups()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new SiteRequest { };
        var folderContext = new FolderPathDataSourceContext { FileDataItemId = "2df3695a-ff87-37fa-7ac7-63d4f4891939" };
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
            Console.WriteLine($"ID: {item.Id}, Display Name: {item.DisplayName}, Type: {(item.Type == 1 ? "File" : "Folder")}");
    }

    [TestMethod]
    public async Task PageFileDataSourceHandler_GetFolderContentAsync_ReturnsComponentIds()
    {
        // Arrange
        var context = GetInvocationContext(ConnectionTypes.OAuth2);
        var request = new SiteRequest { };
        var folderContext = new FolderContentDataSourceContext { FolderId = "68f8b337cbd1cac54f5b9d7f" };
        var handler = new PageFileDataSourceHandler(context, request);

        // Act
        var data = await handler.GetFolderContentAsync(folderContext, CancellationToken.None);

        // Assert
        Assert.IsNotNull(data);

        foreach (var item in data)
            Console.WriteLine($"ID: {item.Id}, Display Name: {item.DisplayName}, Type: {(item.Type == 1 ? "File" : "Folder")}");
    }
}
