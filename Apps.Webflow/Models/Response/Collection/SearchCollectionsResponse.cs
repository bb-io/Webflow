using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Collection;

public record SearchCollectionsResponse(IEnumerable<CollectionEntity> Collections);