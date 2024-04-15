using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Collection;

public class ListCollctionsResponse
{
    public IEnumerable<CollectionEntity> Collections { get; set; }
}