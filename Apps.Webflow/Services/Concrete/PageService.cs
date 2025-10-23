using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.Services.Concrete;

public class PageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.Page;

    public override async Task<SearchContentResponse> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter)
    {
        ThrowForPublishedDateInputs(input, ContentType);

        ValidatorHelper.ValidateInputDates(dateFilter);

        var endpoint = $"sites/{site.SiteId}/pages";
        var request = new RestRequest(endpoint, Method.Get);

        var pages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);

        IEnumerable<PageEntity> filtered = FilterHelper.ApplyDateFilters(pages, dateFilter);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Title) && 
                p.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        var result = filtered.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Title,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }
}
