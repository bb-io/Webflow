namespace Apps.Webflow.Models.Response.Site;

public record SearchSitesResponse(IEnumerable<GetSiteResponse> Sites);