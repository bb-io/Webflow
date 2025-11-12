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

    [Display("Display page metadata", Description = "Display page metadata. Default is true")]
    public bool? DisplayMetadata { get; set; } = true;

    [Display("Include page metadata", Description = "Include translatable metadata in output file. Default is false")]
    public bool? IncludeMetadata { get; set; }

    [Display("Include page slug", Description = "Include slug in output file. Default is false")]
    public bool? IncludeSlug { get; set; } = false;

    [Display("File format", Description = "Format of the file to be downloaded, defaults to an interoperable HTML")]
    [StaticDataSource(typeof(DownloadFileFormatHandler))]
    public string? FileFormat { get; set; }
}
