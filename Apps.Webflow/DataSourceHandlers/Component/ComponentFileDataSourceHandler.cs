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

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(context?.FileDataItemId))
            return new List<FolderPathItem>() { new FolderPathItem() { DisplayName = RootFolderDisplayName, Id = RootFolderId } };

        var result = new List<FolderPathItem>();
        try
        {
            var component = await GetComponentById(context.FileDataItemId);
            var parentFolderId = component.Group;

            while (!string.IsNullOrEmpty(parentFolderId))
            {
                var parentFolder = await GetComponentById(parentFolderId);
                result = result.Prepend(new FolderPathItem()
                {
                    DisplayName = parentFolder.Group ?? "",
                    Id = parentFolder.Group ?? "",
                }).ToList();
                parentFolderId = parentFolder.Group;
            }
            var rootFolder = result.FirstOrDefault();
            if (rootFolder != null)
            {
                rootFolder.DisplayName = RootFolderDisplayName;
                rootFolder.Id = RootFolderId;
            }
        }
        catch (Exception)
        {
            result.Add(new FolderPathItem() { DisplayName = RootFolderDisplayName, Id = RootFolderId });
        }
        return result;
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
