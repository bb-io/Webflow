using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Content;

public record SearchContentResponse(IEnumerable<ContentItemEntity> Items);