using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Services;

namespace Apps.Webflow.Extensions;

public static class ContentServiceExtensions
{
    public static async Task<SearchContentResponse> ExecuteMany(
        this List<IContentService> contentServices,
        SiteRequest site,
        SearchContentRequest request,
        DateFilter dateFilter)
    {
        var result = new List<ContentItemEntity>();

        foreach (var contentService in contentServices)
        {
            var response = (await contentService.SearchContent(site, request, dateFilter)).Result;
            result.AddRange(response);
        }

        return new(result);
    }
}
