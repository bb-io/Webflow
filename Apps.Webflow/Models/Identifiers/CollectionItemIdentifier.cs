using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Identifiers;

public class CollectionItemIdentifier
{
    [Display("Collection item ID"), DataSource(typeof(CollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; } = string.Empty;
}