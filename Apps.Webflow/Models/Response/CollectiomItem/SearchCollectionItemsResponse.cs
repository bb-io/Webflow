using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.CollectiomItem;

public record SearchCollectionItemsResponse(IEnumerable<CollectionItemEntity> Items);