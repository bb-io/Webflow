using RestSharp;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.Services.Concrete;

public class PageService(InvocationContext invocationContext) : BaseContentService<PageEntity>(invocationContext)
{
    public override async Task<IEnumerable<PageEntity>> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter)
    {
        if (input.LastPublishedBefore.HasValue || input.LastPublishedAfter.HasValue)
            throw new PluginMisconfigurationException("'Last published' filter is not supported for pages");

        ValidateInputDates(dateFilter);

        var endpoint = $"sites/{site.SiteId}/pages";
        var request = new RestRequest(endpoint, Method.Get);

        var pages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);

        IEnumerable<PageEntity> filtered = ApplyDateFilters(pages, dateFilter);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Title) && 
                p.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        return filtered;
    }
}
