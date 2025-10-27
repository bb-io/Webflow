using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.Content;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Models.Request.Content;

public class DownloadContentRequest : IDownloadContentInput
{
    [Display("Content ID")]
    [DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionItemCollectionDataSourceHandler))]
    public string? CollectionId { get; set; }

    [Display("Collection item locale ID")]
    [DataSource(typeof(CollectionItemLocaleDataSourceHandler))]
    public string? CmsLocaleId { get; set; }
}
