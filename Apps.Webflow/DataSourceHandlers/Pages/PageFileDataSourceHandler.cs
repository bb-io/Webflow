using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using RestSharp;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;

namespace Apps.Webflow.DataSourceHandlers.Pages;

public class PageFileDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteIdentifier site) 
    : WebflowInvocable(invocationContext), IAsyncFileDataSourceItemHandler
{
    private const string RootFolderDisplayName = "Pages";

    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken token)
    {
        var currentFolderId = string.IsNullOrEmpty(context.FolderId) || context.FolderId == "root"
            ? null
            : context.FolderId;

        var allItems = await ListPages();
        var result = new List<FileDataItem>();

        foreach (var item in allItems)
        {
            if (item.ParentId == currentFolderId)
                result.Add(new File { Id = item.Id, DisplayName = item.Title, Date = item.LastUpdated, IsSelectable = true });
        }

        foreach (var parentId in allItems.Where(x => x.ParentId is not null).GroupBy(x => x.ParentId).Select(g => g.Key))
        {
            var request = new RestRequest($"pages/{parentId}", Method.Get);
            var folder = await Client.ExecuteWithErrorHandling<PageEntity>(request);

            if (folder.ParentId == currentFolderId)
                result.Add(new Folder { Id = folder.Id, DisplayName = folder.Title, Date = folder.LastUpdated, IsSelectable = false });
        }

        return result;
    }

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken token)
    {
        var path = new Stack<FolderPathItem>();

        try
        {
            if (string.IsNullOrEmpty(context?.FileDataItemId) || context.FileDataItemId == "root")
                return new[] { new FolderPathItem { DisplayName = RootFolderDisplayName, Id = "root" } };

            var currentId = context.FileDataItemId;

            while (!string.IsNullOrEmpty(currentId))
            {
                var request = new RestRequest($"pages/{currentId}", Method.Get);
                var page = await Client.ExecuteWithErrorHandling<PageEntity>(request);

                if (page == null)
                    break;

                path.Push(new FolderPathItem
                {
                    Id = page.Id,
                    DisplayName = page.Title
                });

                if (string.IsNullOrEmpty(page.ParentId))
                    break;

                currentId = page.ParentId;
            }

            path.Push(new FolderPathItem
            {
                Id = "root",
                DisplayName = RootFolderDisplayName
            });
        }
        catch
        {
            return new[] { new FolderPathItem { DisplayName = RootFolderDisplayName, Id = "root" } };
        }

        return path;
    }

    private async Task<IEnumerable<PageEntity>> ListPages()
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/pages", Method.Get);
        return await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);
    }
}
