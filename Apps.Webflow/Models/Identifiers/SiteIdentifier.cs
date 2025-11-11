using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Identifiers;

public class SiteIdentifier
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string? SiteId { get; set; }
}