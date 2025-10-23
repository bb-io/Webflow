using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services;

public abstract class BaseContentService(InvocationContext invocationContext) : WebflowInvocable(invocationContext), IContentService
{
    public abstract Task<SearchContentResponse> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter);

    protected static void ThrowForDateInputs(DateFilter date, string contentType)
    {
        if (date.CreatedAfter.HasValue || date.CreatedBefore.HasValue || date.LastUpdatedAfter.HasValue || date.LastUpdatedBefore.HasValue)
            throw new PluginMisconfigurationException($"Date filters are not supported for {contentType}s");
    }

    protected static void ThrowForPublishedDateInputs(SearchContentRequest input, string contentType)
    {
        if (input.LastPublishedBefore.HasValue || input.LastPublishedAfter.HasValue)
            throw new PluginMisconfigurationException($"'Last published' filter is not supported for {contentType}s");
    }
}
