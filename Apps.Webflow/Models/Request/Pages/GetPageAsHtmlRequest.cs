using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Pages;

public class GetPageAsHtmlRequest
{
    [Display("Page ID")]
    [DataSource(typeof(PageDataSourceHandler))]
    public string PageId { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }

    [Display("Include page metadata", Description = "Includes page metadata, true by default")]
    public bool? IncludeMetadata { get; set; }
}
