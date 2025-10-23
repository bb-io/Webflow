using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Webflow.DataSourceHandlers.Collection;

namespace Apps.Webflow.Models.Request.Content;

public class SearchContentRequest
{
    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Last published before")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after")]
    public DateTime? LastPublishedAfter { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionItemCollectionDataSourceHandler))]
    public string? CollectionId { get; set; }
}
