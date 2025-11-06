using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using RestSharp;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;

namespace Apps.Webflow.DataSourceHandlers.Pages;

public class PageFileDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest site) 
    : WebflowInvocable(invocationContext), IAsyncFileDataSourceItemHandler
{
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

    public Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<PageEntity>> ListPages()
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/pages", Method.Get);
        return await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);
    }
}
