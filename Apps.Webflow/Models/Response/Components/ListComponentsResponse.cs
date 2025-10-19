using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Components
{
    public class ListComponentsResponse
    {
        [Display("Components")]
        [JsonProperty("components")]
        public IEnumerable<ComponentResponse> Components { get; set; } = [];

        [JsonProperty("pagination")]
        public PaginationInfo Pagination { get; set; } = new();
    }
}
