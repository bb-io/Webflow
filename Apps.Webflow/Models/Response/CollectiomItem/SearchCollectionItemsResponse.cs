namespace Apps.Webflow.Models.Response.CollectiomItem;

public record SearchCollectionItemsResponse(IEnumerable<GetCollectionItemResponse> Items);