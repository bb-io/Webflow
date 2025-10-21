using Apps.Webflow.Models.Response.Pages;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response;

public class ListPagesResponse
{
    [JsonProperty("pages")]
    public IEnumerable<PageResponse>? Pages { get; set; }

    [JsonProperty("pagination")]
    public PaginationInfo Pagination { get; set; }
}
