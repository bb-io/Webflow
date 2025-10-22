using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Components;

public class SearchComponentsRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; } = string.Empty;

    [Display("Name contains")]
    public string? NameContains { get; set; }

    [Display("Group contains")]
    public string? GroupContains { get; set; }

    [Display("Include read only components")]
    public bool? InludeReadOnly { get; set; }
}
