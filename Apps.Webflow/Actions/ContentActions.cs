using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;

namespace Apps.Webflow.Actions;

[ActionList]
public class ContentActions(InvocationContext invocationContext) : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new ContentServicesFactory(invocationContext);

    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<SearchContentResponse> SearchContent(
        [ActionParameter] ContentFilter contentFilter,
        [ActionParameter] SearchContentRequest request)
    {
        IContentService service = _factory.Create(contentFilter.ContentType);
        return await service.SearchContent(request);
    }
}
