using Apps.Webflow.Constants;
using Apps.Webflow.DataSourceHandlers.Component;
using Apps.Webflow.DataSourceHandlers.Pages;
using Apps.Webflow.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class FileDataSources : TestBaseWithContext
{
    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task ComponentFileDataSourceHandler_GetFolderPathAsync_ReturnsComponentGroups(InvocationContext context)
    {
        // Arrange
        var request = new SiteIdentifier { };
        var folderContext = new FolderPathDataSourceContext { FileDataItemId = "2df3695a-ff87-37fa-7ac7-63d4f4891939" };
        var handler = new ComponentFileDataSourceHandler(context, request);

        // Act
        var data = await handler.GetFolderPathAsync(folderContext, CancellationToken.None);

        // Assert
        PrintResult(data);
        Assert.IsNotNull(data);

        foreach (var item in data)
            Console.WriteLine($"Folder ID: {item.Id}, Display Name: {item.DisplayName}");
    }

    [TestMethod, ContextDataSource(ConnectionTypes.OAuth2)]
    public async Task PageFileDataSourceHandler_GetFolderContentAsync_ReturnsComponentIds(InvocationContext context)
    {
        // Arrange
        var request = new SiteIdentifier { };
        var folderContext = new FolderContentDataSourceContext { FolderId = "690b3264e2ac9b34a3109595" };
        var handler = new PageFileDataSourceHandler(context, request);

        // Act
        var data = await handler.GetFolderContentAsync(folderContext, CancellationToken.None);

        // Assert
        Assert.IsNotNull(data);

        foreach (var item in data)
            Console.WriteLine($"ID: {item.Id}, Display Name: {item.DisplayName}, Type: {(item.Type == 1 ? "File" : "Folder")}");
    }
}
