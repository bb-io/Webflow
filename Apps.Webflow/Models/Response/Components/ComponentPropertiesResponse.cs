using Apps.Webflow.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Components;

public class ComponentPropertiesResponse
{
    [JsonProperty("componentId")]
    public string ComponentId { get; set; }

    [JsonProperty("properties")]
    public List<ComponentPropertyEntity> Properties { get; set; } = new List<ComponentPropertyEntity>();

    [JsonProperty("pagination")]
    public PaginationInfo? Pagination { get; set; }
}

public class ComponentPropertyEntity
{
    [JsonProperty("propertyId")]
    public string PropertyId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("text")]
    public ComponentTextContent Text { get; set; }
}
