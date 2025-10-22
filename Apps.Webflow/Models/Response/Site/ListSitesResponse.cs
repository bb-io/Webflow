using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Site;

public class ListSitesResponse
{
    public IEnumerable<SiteEntity> Sites { get; set; }
}