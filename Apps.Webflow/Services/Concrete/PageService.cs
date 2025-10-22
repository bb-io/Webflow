using Apps.Webflow.Constants;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pages;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.Services.Concrete;

public class PageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<SearchContentResponse> SearchContent(SearchContentRequest input)
    {
        var allPages = new List<PageResponse>();
        var offset = 0;
        const int pageSize = 100;
        int total = int.MaxValue;

        while (allPages.Count < total)
        {
            var endpoint = $"sites/{input.SiteId}/pages";
            var request = new RestRequest(endpoint, Method.Get);

            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("limit", pageSize.ToString());

            var batch = await Client.ExecuteWithErrorHandling<ListPagesResponse>(request);

            var batchPages = batch.Pages?.ToList() ?? new List<PageResponse>();
            total = batch.Pagination?.Total ?? batchPages.Count;

            allPages.AddRange(batchPages);

            if (batchPages.Count == 0) break;
            offset += batchPages.Count;
        }

        var result = allPages.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Title,
            Type = ContentTypes.Page
        });
        return new SearchContentResponse(result);
    }
}
