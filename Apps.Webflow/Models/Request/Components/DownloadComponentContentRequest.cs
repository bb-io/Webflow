using Apps.Webflow.DataSourceHandlers.Component;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Components;

public class DownloadComponentContentRequest
{
    [Display("Component ID")]
    [FileDataSource(typeof(ComponentFileDataSourceHandler))]
    public string ComponentId { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}
