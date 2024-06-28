using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request;

public class SiteLocaleRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }
    
    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}