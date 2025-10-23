using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Pages;

public class ListPagesResponse
{
    [JsonProperty("pages")]
    public IEnumerable<PageEntity>? Pages { get; set; }

    [JsonProperty("pagination")]
    public PaginationInfo Pagination { get; set; }
}
