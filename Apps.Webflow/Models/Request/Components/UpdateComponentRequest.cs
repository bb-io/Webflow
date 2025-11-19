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

    [JsonProperty("placeholder", NullValueHandling = NullValueHandling.Ignore)]
    public string? Placeholder { get; set; }

    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value { get; set; }

    [JsonProperty("waitingText", NullValueHandling = NullValueHandling.Ignore)]
    public string? WaitingText { get; set; }

    [JsonProperty("choices", NullValueHandling = NullValueHandling.Ignore)]
    public List<SelectChoiceUpdate>? Choices { get; set; }
}

public class ComponentPropertyOverride
{
    [JsonProperty("propertyId")]
    public string PropertyId { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}

public class SelectChoiceUpdate
{
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}

public class UpdateComponentPropertiesRequest
{
    [JsonProperty("properties")]
    public IEnumerable<UpdateComponentProperty> Properties { get; set; } = [];
}

public class UpdateComponentProperty
{
    [JsonProperty("propertyId")]
    public string PropertyId { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}
