using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;

namespace Apps.Webflow.Services;

public interface IContentService
{
    Task<SearchContentResponse> SearchContent(SearchContentRequest input);
}
