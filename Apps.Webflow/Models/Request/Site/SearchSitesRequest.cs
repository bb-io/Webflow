using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Site;

public class SearchSitesRequest
{
    [Display("Display name contains")]
    public string? DisplayNameContains { get; set; }

    [Display("Last published before")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after")]
    public DateTime? LastPublishedAfter { get; set; }
}
