using Apps.Webflow.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Collection;

public class CollectionRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }
    
    [Display("Collection ID")]
    [DataSource(typeof(CollectionDataSourceHandler))]
    public string CollectionId { get; set; }
}