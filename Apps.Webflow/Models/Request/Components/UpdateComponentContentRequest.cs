using Apps.Webflow.DataSourceHandlers.Component;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Components;

public class UpdateComponentContentRequest
{
    [Display("Component")]
    public FileReference File { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }

    [Display("Component ID")]
    [FileDataSource(typeof(ComponentDataSourceHandler))]
    public string? ComponentId { get; set; }
}
