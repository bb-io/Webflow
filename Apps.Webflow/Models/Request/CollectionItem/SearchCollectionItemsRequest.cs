using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class SearchCollectionItemsRequest
{
    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Last published before (for collection items)")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after (for collection items)")]
    public DateTime? LastPublishedAfter { get; set; }
}
