using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Pages;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Pages;

public class DownloadPageRequest
{
    [Display("Page ID")]
    [FileDataSource(typeof(PageFileDataSourceHandler))]
    public string PageId { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }

    [Display("Include page metadata", Description = "Includes page metadata, true by default")]
    public bool? IncludeMetadata { get; set; }
}
