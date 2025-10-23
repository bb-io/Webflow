using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;

namespace Apps.Webflow.Services;

public interface IContentService<T>
{
    Task<IEnumerable<T>> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter);
}
