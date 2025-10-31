using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Polling.Models.Requests;

public class PageUpdatedRequest
{
    [Display("Page name contains")]
    public string? NameContains { get; set; }
}
