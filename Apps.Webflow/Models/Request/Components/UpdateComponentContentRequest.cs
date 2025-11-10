using Apps.Webflow.DataSourceHandlers.Component;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Components;

public class UpdateComponentContentRequest
{
    [Display("Component")]
    public FileReference File { get; set; }

    [Display("Component ID")]
    [FileDataSource(typeof(ComponentFileDataSourceHandler))]
    public string? ComponentId { get; set; }
}
