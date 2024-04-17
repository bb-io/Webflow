using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class CollectionItemRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }
    
    [Display("Collection ID")]
    [DataSource(typeof(CollectionItemCollectionDataSourceHandler))]
    public string CollectionId { get; set; }
    
    [Display("Locale")]
    [DataSource(typeof(CollectionItemLocaleDataSourceHandler))]
    public string CmsLocaleId { get; set; }
    
    [Display("Collection item ID")]
    [DataSource(typeof(CollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; }
}