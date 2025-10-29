using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class PublishItemRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }
    
    [Display("Collection ID")]
    [DataSource(typeof(PublishItemRequestCollectionDataSourceHandler))]
    public string CollectionId { get; set; }
    
    [Display("Collection item ID")]
    [DataSource(typeof(PublishCollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; }
}