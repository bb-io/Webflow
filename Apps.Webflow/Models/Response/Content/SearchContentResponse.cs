using Apps.Webflow.Models.Entities.Content;

namespace Apps.Webflow.Models.Response.Content;

public record SearchContentResponse(IEnumerable<ContentItemEntity> Items);