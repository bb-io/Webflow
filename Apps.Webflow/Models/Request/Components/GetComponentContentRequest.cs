using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Components;

public class GetComponentContentRequest
{
    [Display("Component ID")]
    [DataSource(typeof(ComponentDataSourceHandler))]
    public string ComponentId { get; set; } = string.Empty;

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}
