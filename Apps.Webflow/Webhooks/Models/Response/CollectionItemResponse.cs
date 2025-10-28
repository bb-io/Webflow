using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionItemResponse : IDownloadContentInput
{
    [Display("Collection item ID")] public string ContentId { get; set; }
    
    [Display("Site ID")] public string SiteId { get; set; }
    
    [Display("Collection ID")] public string CollectionId { get; set; }
    
    [Display("Workspace ID")] public string WorkspaceId { get; set; }
}