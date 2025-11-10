using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class PublishItemRequest
{    
    [Display("Collection ID")]
    [DataSource(typeof(PublishItemRequestCollectionDataSourceHandler))]
    public string CollectionId { get; set; }
    
    [Display("Collection item ID")]
    [DataSource(typeof(PublishCollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; }

    [Display("Locales")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public IEnumerable<string>? CmsLocales { get; set; }
}