using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;

namespace Apps.Webflow.Services;

public interface IContentService
{
    Task<SearchContentResponse> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter);
    Task<DownloadContentResponse> DownloadContent(string id);
}
