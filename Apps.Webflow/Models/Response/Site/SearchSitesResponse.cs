namespace Apps.Webflow.Models.Response.Site;

public class SearchSitesResponse(IEnumerable<GetSiteResponse> sites)
{
    public IEnumerable<GetSiteResponse> Sites { get; set; } = sites;
}