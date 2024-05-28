using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities;

public class FullCollectionEntity : CollectionEntity
{
    [Display("Collection items")]
    public IEnumerable<CollectionItemEntity> CollectionItems { get; set; }
}