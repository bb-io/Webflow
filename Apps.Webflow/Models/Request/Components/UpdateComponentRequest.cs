using Newtonsoft.Json;

namespace Apps.Webflow.Models.Request.Components;

public class UpdateComponentDomRequest
{
    [JsonProperty("nodes")]
    public IEnumerable<UpdateComponentNode> Nodes { get; set; } = [];
}

public class UpdateComponentNode
{
    [JsonProperty("nodeId")]
    public string NodeId { get; set; } = string.Empty;

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string? Text { get; set; }

    [JsonProperty("propertyOverrides", NullValueHandling = NullValueHandling.Ignore)]
    public List<ComponentPropertyOverride>? PropertyOverrides { get; set; }
}

public class ComponentPropertyOverride
{
    [JsonProperty("propertyId")]
    public string PropertyId { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}
