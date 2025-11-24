using Newtonsoft.Json;

namespace Apps.Webflow.Models.Request.Pages;

public class UpdatePageDomRequest
{
    [JsonProperty("nodes")]
    public required IEnumerable<UpdatePageNode> Nodes { get; set; }
}

public class UpdatePageNode
{
    [JsonProperty("nodeId")]
    public required string NodeId { get; set; }

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string? Text { get; set; }

    [JsonProperty("placeholder", NullValueHandling = NullValueHandling.Ignore)]
    public string? Placeholder { get; set; }

    [JsonProperty("propertyOverrides", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<PropertyOverride>? PropertyOverrides { get; set; }
}

public class PropertyOverride
{
    [JsonProperty("propertyId")]
    public required string PropertyId { get; set; }

    [JsonProperty("text")]
    public required string Text { get; set; }
}
