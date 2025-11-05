using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Response.Pages;
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
        var allItems = await ListPages();
        var result = new List<FileDataItem>();
        var currentFolder = context.FolderId ?? "/";

        foreach (var item in allItems)
        {
            if (item.ParentId != null)
            {
                var page = await GetPage(item.ParentId);
                var path = page.PublishedPath?.TrimEnd('/') ?? "/";
                var lastSlashIndex = path.LastIndexOf('/');
                bool hasNothingAfter = lastSlashIndex == 0;
                if (hasNothingAfter)
                    result.Add(new Folder { Id = path, DisplayName = page.Title, Date = page.LastUpdated });
            }
            else
                result.Add(new File { Id = item.Id, DisplayName = item.Title, Date = item.LastUpdated });
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
        return (await Client.ExecuteWithErrorHandling<SearchPagesResponse>(request)).Pages;
    }

    private async Task<PageEntity> GetPage(string pageId)
    {
        var request = new RestRequest($"pages/{pageId}", Method.Get);
        return await Client.ExecuteWithErrorHandling<PageEntity>(request);
    }
}
