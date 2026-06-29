using Newtonsoft.Json;

namespace Apps.Webflow.Models.Entities.Asset;

public class UploadAssetEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("uploadUrl")]
    public string UploadUrl { get; set; } = string.Empty;
    
    [JsonProperty("hostedUrl")]
    public string HostedUrl { get; set; } = string.Empty;

    [JsonProperty("uploadDetails")]
    public UploadDetails UploadDetails { get; set; } = null!;
}