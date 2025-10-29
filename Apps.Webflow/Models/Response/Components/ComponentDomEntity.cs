using Apps.Webflow.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Components;

public class ComponentDomEntity
{
    [JsonProperty("componentId")]
    public string ComponentId { get; set; }

    [JsonProperty("nodes")]
    public List<ComponentNodeEntity> Nodes { get; set; } = new List<ComponentNodeEntity>();

    [JsonProperty("pagination")]
    public PaginationInfo? Pagination { get; set; }
}

public class ComponentNodeEntity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("attributes")]
    public Dictionary<string, string>? Attributes { get; set; }

    [JsonProperty("text")]
    public ComponentTextContent? Text { get; set; }

    [JsonProperty("componentId")]
    public string? ComponentId { get; set; }

    [JsonProperty("propertyOverrides")]
    public List<ComponentPropertyOverrideEntity>? PropertyOverrides { get; set; }

    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    [JsonProperty("choices")]
    public List<SelectChoice>? Choices { get; set; }

    [JsonProperty("value")]
    public string? Value { get; set; }

    [JsonProperty("waitingText")]
    public string? WaitingText { get; set; }
}

public class ComponentTextContent
{
    [JsonProperty("html")]
    public string? Html { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }
}

public class ComponentPropertyOverrideEntity
{
    [JsonProperty("propertyId")]
    public string PropertyId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("label")]
    public string? Label { get; set; }

    [JsonProperty("text")]
    public ComponentTextContent Text { get; set; }
}

public class SelectChoice
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
}
