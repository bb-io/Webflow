using Apps.Webflow.Models.Entities.CollectionItem;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Response.CollectiomItem;

public class GetCollectionItemResponse(CollectionItemEntity entity)
{
    [Display("Collection item ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Locale")]
    public string? Locale { get; set; } = entity.CmsLocaleId;

    [Display("Name")]
    public string? Name { get; set; } = entity.Name;

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; } = entity.LastUpdated;

    [Display("Last published")]
    public DateTime? LastPublished { get; set; } = entity.LastPublished;

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; } = entity.CreatedOn;
}
