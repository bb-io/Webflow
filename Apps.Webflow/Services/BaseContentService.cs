using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services;

public abstract class BaseContentService(InvocationContext invocationContext) : WebflowInvocable(invocationContext), IContentService
{
    public abstract Task<SearchContentResponse> SearchContent(SearchContentRequest input);
}
