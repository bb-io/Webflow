using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Identifiers;

public class LocaleIdentifier
{
    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? Locale { get; set; }
}
