using RestSharp;
using Apps.Webflow.Constants;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services.Concrete;

public class ComponentService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.Component;

    public async override Task<SearchContentResponse> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter)
    {
        ThrowForDateInputs(dateFilter, ContentType);
        ThrowForPublishedDateInputs(input, ContentType);

        var endpoint = $"sites/{site.SiteId}/components";
        var request = new RestRequest(endpoint, Method.Get);

        IEnumerable<ComponentEntity> pages = await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(request, r => r.Components);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            pages = pages.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        var result = pages.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Name,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }

    public override async Task<Stream> DownloadContent(SiteRequest site, DownloadContentRequest input)
    {
        throw new NotImplementedException();
    }
}
