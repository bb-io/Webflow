using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Components;

public class GetComponentContentRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; } = string.Empty;

    [Display("Component ID")]
    [DataSource(typeof(ComponentDataSourceHandler))]
    public string ComponentId { get; set; } = string.Empty;

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}
