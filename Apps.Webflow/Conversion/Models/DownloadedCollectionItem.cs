using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Conversion.Models;

public class DownloadedCollectionItem
{
    public string? CmsLocaleId { get; set; }
    public required string CollectionItemId { get; set; }
    public required string CollectionId { get; set; }
    public required string SiteId { get; set; }
    public required CollectionItemEntity CollectionItem { get; set; }
}
