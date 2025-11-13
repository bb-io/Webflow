using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities.Collection;

public class CollectionEntity : IDateFilterable
{
    [Display("Collection ID")]
    public string Id { get; set; }
    
    [Display("Display name")]
    public string DisplayName { get; set; }
    
    [Display("Singular name")]
    public string SingularName { get; set; }

    [Display("Slug")]
    public string? Slug { get; set; }
    
    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; }

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; }

    public IEnumerable<FieldEntity> Fields { get; set; }
}