using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Services;

namespace Apps.Webflow.Extensions;

public static class ContentServiceExtensions
{
    public static async Task<SearchContentResponse> ExecuteMany(
        this List<IContentService> contentServices,
        string siteId,
        SearchContentRequest request,
        ContentDateFilter dateFilter)
    {
        var result = new List<ContentItemEntity>();

        foreach (var contentService in contentServices)
        {
            var response = (await contentService.SearchContent(siteId, request, dateFilter)).Items;
            result.AddRange(response);
        }

        return new(result);
    }
}
