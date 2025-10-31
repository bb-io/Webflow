using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Polling.Models.Requests;

public class PageUpdatedRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }

    [Display("Page name contains")]
    public string? NameContains { get; set; }
}
