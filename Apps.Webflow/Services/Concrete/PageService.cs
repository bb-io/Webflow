using RestSharp;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services.Concrete;

public class PageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<SearchContentResponse> SearchContent(SearchContentRequest input)
    {
        var endpoint = $"sites/{input.SiteId}/pages";
        var request = new RestRequest(endpoint, Method.Get);

        var pages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);

        var result = pages.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Title,
            Type = ContentTypes.Page
        });
        return new SearchContentResponse(result);
    }
}
