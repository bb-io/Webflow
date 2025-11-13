using Apps.Webflow.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Entities.CollectionItem;

namespace Apps.Webflow.Conversion.Models;

public class DownloadedCollectionItem
{
    public string contentType = ContentTypes.CollectionItem.ToKebabCase();

    public string? Locale { get; set; }
    public required string CollectionId { get; set; }
    public required string SiteId { get; set; }
    public required CollectionItemEntity CollectionItem { get; set; }
}
