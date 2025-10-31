using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Webhooks.Models.Request;

public class CollectionItemDeletedWebhookRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionDataSourceHandler))]
    public string? CollectionId { get; set; }
}
