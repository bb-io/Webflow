using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Content;

public class SearchContentRequest
{
    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Last published before")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after")]
    public DateTime? LastPublishedAfter { get; set; }
}
