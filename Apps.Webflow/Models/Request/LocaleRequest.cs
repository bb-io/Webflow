using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request;

public class LocaleRequest
{
    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? Locale { get; set; }
}
