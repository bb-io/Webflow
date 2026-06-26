using Newtonsoft.Json;

namespace Apps.Webflow.Models.Entities.Asset;

public class UploadDetails
{
    [JsonProperty("acl")]
    public string Acl { get; set; } = string.Empty;

    [JsonProperty("bucket")]
    public string Bucket { get; set; } = string.Empty;

    [JsonProperty("X-Amz-Algorithm")]
    public string XAmzAlgorithm { get; set; } = string.Empty;

    [JsonProperty("X-Amz-Credential")]
    public string XAmzCredential { get; set; } = string.Empty;

    [JsonProperty("X-Amz-Date")]
    public string XAmzDate { get; set; } = string.Empty;

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("Policy")]
    public string Policy { get; set; } = string.Empty;

    [JsonProperty("X-Amz-Signature")]
    public string XAmzSignature { get; set; } = string.Empty;

    [JsonProperty("success_action_status")]
    public string SuccessActionStatus { get; set; } = string.Empty;

    [JsonProperty("content-type")]
    public string ContentType { get; set; } = string.Empty;

    [JsonProperty("Cache-Control")] 
    public string CacheControl { get; set; } = string.Empty;
    
    public IEnumerable<KeyValuePair<string, string>> ToFormFields() =>
    [
        new("acl", Acl),
        new("bucket", Bucket),
        new("X-Amz-Algorithm", XAmzAlgorithm),
        new("X-Amz-Credential", XAmzCredential),
        new("X-Amz-Date", XAmzDate),
        new("key", Key),
        new("Policy", Policy),
        new("X-Amz-Signature", XAmzSignature),
        new("success_action_status", SuccessActionStatus),
        new("content-type", ContentType),
        new("Cache-Control", CacheControl)
    ];
}