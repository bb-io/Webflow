using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities;

public class CollectionEntity
{
    [Display("Collection ID")]
    public string Id { get; set; }
    
    [Display("Display name")]
    public string DisplayName { get; set; }
    
    [Display("Singular name")]
    public string SingularName { get; set; }
    
    public string? Slug { get; set; }
    
    [Display("Last updated")]
    public DateTime LastUpdated { get; set; }
    
    public IEnumerable<FieldEntity> Fields { get; set; }
}