using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Collection;

public class CreateCollectionRequest
{
    [Display("Display name")]
    public string DisplayName { get; set; }
    
    [Display("Singular name")]
    public string SingularName { get; set; }
    
    public string? Slug { get; set; }
}