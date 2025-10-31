using Apps.Webflow.DataSourceHandlers.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Collection;

public class CollectionRequest
{
    [Display("Collection ID")]
    [DataSource(typeof(CollectionDataSourceHandler))]
    public string CollectionId { get; set; }
}