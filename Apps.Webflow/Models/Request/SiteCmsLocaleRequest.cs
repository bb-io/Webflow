using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request;

public class SiteCmsLocaleRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }
    
    [Display("Locale")]
    [DataSource(typeof(SiteCmsLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}