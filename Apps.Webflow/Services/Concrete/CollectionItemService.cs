using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.Services.Concrete;

public class CollectionItemService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.CollectionItem;

    public async override Task<SearchContentResponse> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter)
    {
        if (string.IsNullOrEmpty(input.CollectionId))
            throw new PluginMisconfigurationException("Please specify collection ID in order to search content items");

        ValidatorHelper.ValidateInputDates(dateFilter);
        ValidatorHelper.ValidatePublishedInputDates(input.LastPublishedBefore, input.LastPublishedAfter);

        var endpoint = $"collections/{input.CollectionId}/items";
        var request = new RestRequest(endpoint, Method.Get);

        var items = await Client.Paginate<CollectionItemEntity, CollectionItemPaginationResponse>(request, r => r.Items);
        IEnumerable<CollectionItemEntity> filtered = FilterHelper.ApplyDateFilters(items, dateFilter);

        if (input.LastPublishedBefore.HasValue)
            filtered = filtered.Where(c => c.LastPublished <= input.LastPublishedBefore);

        if (input.LastPublishedAfter.HasValue)
            filtered = filtered.Where(c => c.LastPublished >= input.LastPublishedAfter);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            filtered = filtered.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        var result = filtered.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Name,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }

    public override Task<DownloadContentResponse> DownloadContent(string id)
    {
        throw new NotImplementedException();
    }
}
