using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Components;

public class ListComponentsResponse
{
    [Display("Components")]
    [JsonProperty("components")]
    public IEnumerable<ComponentEntity> Components { get; set; } = [];

    [JsonProperty("pagination")]
    public PaginationInfo Pagination { get; set; } = new();
}
