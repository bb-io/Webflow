using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Site;

public class SearchSitesRequest
{
    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Display name contains")]
    public string? DisplayNameContains { get; set; }

    [Display("Last published before")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after")]
    public DateTime? LastPublishedAfter { get; set; }

    [Display("Last updated before")]
    public DateTime? LastUpdatedBefore { get; set; }

    [Display("Last updated after")]
    public DateTime? LastUpdatedAfter { get; set; }
}
