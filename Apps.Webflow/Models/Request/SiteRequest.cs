using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request;

public class SiteRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string? SiteId { get; set; }
}