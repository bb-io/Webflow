namespace Apps.Webflow.Models.Response.Site;

public class ListSitesResponse(IEnumerable<GetSiteResponse> sites)
{
    public IEnumerable<GetSiteResponse> Sites { get; set; } = sites;
}