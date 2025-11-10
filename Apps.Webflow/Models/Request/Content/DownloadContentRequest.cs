using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.Content;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Handlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Models.Request.Content;

public class DownloadContentRequest : IDownloadContentInput
{
    [Display("Content ID")]
    [DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; }

    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? Locale { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionItemCollectionDataSourceHandler))]
    public string? CollectionId { get; set; }

    [Display("File format", Description = "Format of the file to be downloaded, defaults to an interoperable HTML")]
    [StaticDataSource(typeof(DownloadFileFormatHandler))]
    public string? FileFormat { get; set; }
}
