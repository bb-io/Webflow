using Apps.Webflow.Models.Entities.Collection;

namespace Apps.Webflow.Models.Response.Collection;

public record SearchCollectionsResponse(IEnumerable<CollectionEntity> Collections);