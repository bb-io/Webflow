using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Pages;

public class SearchPagesRequest
{
    [Display("Locale ID")]
    public string? LocaleId { get; set; }

    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Slug contains")]
    public string? SlugContains { get; set; }

    [Display("Archived")]
    public bool? Archived { get; set; }

    [Display("Is draft")]
    public bool? Draft { get; set; }
}
