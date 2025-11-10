using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class PublishItemRequest
{   
    [Display("Collection item ID")]
    [DataSource(typeof(CollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; }
}