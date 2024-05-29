using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Collection;

public class ListCollectionsResponse
{
    public IEnumerable<CollectionEntity> Collections { get; set; }
}