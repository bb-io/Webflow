using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Webhooks.Models.Request;

public class CollectionItemWebhookRequest
{
    [Display("Locale ID")]
    [DataSource(typeof(SiteCmsLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionDataSourceHandler))]
    public string? CollectionId { get; set; }
}
