using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Response.Components;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using RestSharp;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;

namespace Apps.Webflow.DataSourceHandlers.Component;

public class ComponentFileDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest site) 
    : WebflowInvocable(invocationContext), IAsyncFileDataSourceItemHandler
{
    private const string RootFolderDisplayName = "Components";
    private const string RootFolderId = "Root";

    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken token)
    {
        var result = new List<FileDataItem>();
        var sourceItems = await ListItemsInFolderById(string.IsNullOrEmpty(context.FolderId) ? RootFolderId : context.FolderId);

        foreach (var item in sourceItems)
        {
            if (!string.IsNullOrEmpty(item.Group) && item.Group != context.FolderId)
            {
                result.Add(new Folder()
                {
                    Id = item.Group,
                    DisplayName = item.Group,
                    IsSelectable = false
                });
            } 
            else
            {
                result.Add(new File()
                {
                    Id = item.Id,
                    DisplayName = item.Name,
                    IsSelectable = true
                });
            }
        }

        return result;
    }

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken token)
    {
        if (string.IsNullOrEmpty(context?.FileDataItemId))
            return new[] { new FolderPathItem { DisplayName = RootFolderDisplayName, Id = RootFolderId } };

        var path = new Stack<FolderPathItem>();

        try
        {
            var currentId = context.FileDataItemId;

            while (!string.IsNullOrEmpty(currentId))
            {
                var component = await GetComponentById(currentId);

                path.Push(new FolderPathItem
                {
                    Id = currentId,
                    DisplayName = component.Name ?? component.Group ?? string.Empty
                });

                currentId = component.Group;
            }

            path.Push(new FolderPathItem
            {
                Id = RootFolderId,
                DisplayName = RootFolderDisplayName
            });
        }
        catch
        {
            return new[] { new FolderPathItem {  DisplayName = RootFolderDisplayName, Id = RootFolderId }};
        }

        return path;
    }

    private async Task<IEnumerable<ComponentEntity>> ListItemsInFolderById(string? folderId)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/components", Method.Get);
        var components = await Client.ExecuteWithErrorHandling<SearchComponentsResponse>(request);

        if (folderId == RootFolderId)
            return components.Components;

        return components.Components.Where(x => x.Group == folderId).ToList();
    }

    public async Task<ComponentEntity> GetComponentById(string componentId)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/components", Method.Get);
        var components = await Client.ExecuteWithErrorHandling<SearchComponentsResponse>(request);
        return components.Components.First(x => x.Id == componentId);
    }
}
