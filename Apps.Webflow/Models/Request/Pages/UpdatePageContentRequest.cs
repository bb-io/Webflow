using Apps.Webflow.DataSourceHandlers.Pages;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Pages;

public class UpdatePageContentRequest
{
    [Display("Page")]
    public FileReference File { get; set; }

    [Display("Page ID")]
    [FileDataSource(typeof(PageFileDataSourceHandler))]
    public string? PageId { get; set; }
}
