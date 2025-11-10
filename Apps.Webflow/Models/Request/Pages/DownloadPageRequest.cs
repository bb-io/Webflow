using Apps.Webflow.DataSourceHandlers.Pages;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.SDK.Blueprints.Handlers;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Pages;

public class DownloadPageRequest
{
    [Display("Page ID")]
    [FileDataSource(typeof(PageFileDataSourceHandler))]
    public string PageId { get; set; }

    [Display("Include page metadata", Description = "Includes page metadata, true by default")]
    public bool? IncludeMetadata { get; set; }

    [Display("File format", Description = "Format of the file to be downloaded, defaults to an interoperable HTML")]
    [StaticDataSource(typeof(DownloadFileFormatHandler))]
    public string? FileFormat { get; set; }
}
