using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionItemResponse
{
    [Display("Collection item ID")] public string Id { get; set; }
    
    [Display("Site ID")] public string SiteId { get; set; }
    
    [Display("Collection ID")] public string CollectionId { get; set; }
    
    [Display("Workspace ID")] public string WorkspaceId { get; set; }
}