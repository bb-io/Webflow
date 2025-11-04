using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Polling.Models.Requests;

public class PageUpdatedRequest
{
    [Display("Page name contains")]
    public string? NameContains { get; set; }

    [Display("Page name doesn't contain")]
    public string? NameDoesNotContain { get; set; }
}
