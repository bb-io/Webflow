using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Webflow.Models.Request.Pages;

public class UpdatePageContentRequest
{
    [Display("Page")]
    public FileReference File { get; set; }

    [Display("Page ID")]
    [DataSource(typeof(PageDataSourceHandler))]
    public string? PageId { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}
