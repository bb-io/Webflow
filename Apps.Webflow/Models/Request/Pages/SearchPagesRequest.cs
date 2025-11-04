using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Pages;

public class SearchPagesRequest
{
    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Slug contains")]
    public string? SlugContains { get; set; }

    [Display("Published path contains")]
    public string? PublishedPathContains { get; set; }

    [Display("Archived")]
    public bool? Archived { get; set; }

    [Display("Is draft")]
    public bool? Draft { get; set; }
}
