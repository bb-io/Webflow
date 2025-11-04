using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Polling.Models.Requests;

public class PagePollingRequest
{
    [Display("Page name contains")]
    public string? NameContains { get; set; }

    [Display("Page name doesn't contain")]
    public string? NameDoesNotContain { get; set; }

    [Display("Slug contains")]
    public string? SlugContains { get; set; }

    [Display("Published path contains")]
    public string? PublishedPathContains { get; set; }
}
