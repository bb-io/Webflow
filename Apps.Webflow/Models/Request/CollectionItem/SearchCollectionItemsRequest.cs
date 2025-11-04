using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class SearchCollectionItemsRequest
{
    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Slug contains")]
    public string? SlugContains { get; set; }

    [Display("Locale ID")]
    [DataSource(typeof(CollectionItemLocaleDataSourceHandler))]
    public string? CmsLocaleId { get; set; }

    [Display("Last published before")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after")]
    public DateTime? LastPublishedAfter { get; set; }
}
