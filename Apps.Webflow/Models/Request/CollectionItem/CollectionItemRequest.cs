using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Handlers;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class CollectionItemRequest
{    
    [Display("Collection ID")]
    [DataSource(typeof(CollectionItemCollectionDataSourceHandler))]
    public string CollectionId { get; set; }
    
    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? CmsLocale { get; set; }
    
    [Display("Collection item ID")]
    [DataSource(typeof(CollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; }

    [Display("File format", Description = "Format of the file to be downloaded, defaults to an interoperable HTML")]
    [StaticDataSource(typeof(DownloadFileFormatHandler))]
    public string? FileFormat { get; set; }
}