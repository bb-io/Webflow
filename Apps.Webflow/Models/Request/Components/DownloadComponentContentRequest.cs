using Apps.Webflow.DataSourceHandlers.Component;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Handlers;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Webflow.Models.Request.Components;

public class DownloadComponentContentRequest
{
    [Display("Component ID")]
    [FileDataSource(typeof(ComponentFileDataSourceHandler))]
    public string ComponentId { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }

    [Display("File format", Description = "Format of the file to be downloaded, defaults to an interoperable HTML")]
    [StaticDataSource(typeof(DownloadFileFormatHandler))]
    public string? FileFormat { get; set; }
}
