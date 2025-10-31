using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Newtonsoft.Json;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionItemResponse : IDownloadContentInput
{
    [JsonProperty("id")]
    [Display("Collection item ID")] 
    public string ContentId { get; set; }
    
    [JsonProperty("siteId")]
    [Display("Site ID")]
    public string SiteId { get; set; }
    
    [JsonProperty("collectionId")]
    [Display("Collection ID")]
    public string CollectionId { get; set; }
    
    [JsonProperty("workspaceId")]
    [Display("Workspace ID")]
    public string WorkspaceId { get; set; }

    [JsonProperty("cmsLocaleId")]
    [Display("Locale ID")]
    public string CmsLocaleId { get; set; }
}