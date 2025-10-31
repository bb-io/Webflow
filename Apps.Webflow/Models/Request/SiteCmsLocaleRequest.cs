using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request;

public class SiteCmsLocaleRequest
{    
    [Display("Locale ID")]
    [DataSource(typeof(SiteCmsLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}