using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Components;

public class ComponentResponse
{
    [Display("Component ID")]
    [JsonProperty("id")]
    public string Id { get; set; }

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [Display("Group")]
    [JsonProperty("group")]
    public string? Group { get; set; }

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Read only")]
    [JsonProperty("readonly")]
    public bool? ReadOnly { get; set; }
}
