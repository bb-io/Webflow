using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Handlers;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class DownloadCollectionItemRequest
{        
    [Display("Collection item ID")]
    [DataSource(typeof(CollectionItemDataSourceHandler))]
    public string CollectionItemId { get; set; }

    [Display("File format", Description = "Format of the file to be downloaded, defaults to an interoperable HTML")]
    [StaticDataSource(typeof(DownloadFileFormatHandler))]
    public string? FileFormat { get; set; }

    [Display("Include collection item slug", Description = "Include slug in output file. Default is false")]
    public bool? IncludeSlug { get; set; }
}