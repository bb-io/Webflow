using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Webhooks.Models.Request;

public class CollectionItemWebhookRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string? SiteId { get; set; }

    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? Locale { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionDataSourceHandler))]
    public string? CollectionId { get; set; }
}
