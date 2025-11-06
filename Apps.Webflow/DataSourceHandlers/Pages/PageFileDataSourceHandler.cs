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
        var currentFolder = context.FolderId ?? "";
        var allItems = await ListPages(currentFolder);
        var result = new List<FileDataItem>();

        foreach (var item in allItems)
        {
            if (item.ParentId != null)
            {
                var page = await GetPage(item.ParentId);
                var path = page.PublishedPath?.TrimEnd('/') ?? "/";
                var lastSlashIndex = path.LastIndexOf('/');
                bool hasNothingAfter = lastSlashIndex == 0;
                if (hasNothingAfter)
                    result.Add(new Folder { Id = item.Id, DisplayName = page.Title, Date = page.LastUpdated, IsSelectable = false });
            }
            else
                result.Add(new File { Id = item.Id, DisplayName = item.Title, Date = item.LastUpdated, IsSelectable = true });
        }

        return result;
    }

    public Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<PageEntity>> ListPages(string parentId)
    {
        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/pages", Method.Get);
        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);
        return response;
    }

    private async Task<PageEntity> GetPage(string pageId)
    {
        var request = new RestRequest($"pages/{pageId}", Method.Get);
        return await Client.ExecuteWithErrorHandling<PageEntity>(request);
    }
}
