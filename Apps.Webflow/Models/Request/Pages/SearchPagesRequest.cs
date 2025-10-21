using Apps.Webflow.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Pages;

public class SearchPagesRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string SiteId { get; set; }

    [Display("Locale ID")]
    public string? LocaleId { get; set; }

    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Slug contains")]
    public string? SlugContains { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Last updated after")]
    public DateTime? LastUpdatedAfter { get; set; }

    [Display("Last updated before")]
    public DateTime? LastUpdatedBefore { get; set; }

    [Display("Archived")]
    public bool? Archived { get; set; }

    [Display("Is draft")]
    public bool? Draft { get; set; }
}
