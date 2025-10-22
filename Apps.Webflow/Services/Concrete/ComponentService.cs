using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services.Concrete;

public class ComponentService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override Task<SearchContentResponse> SearchContent(SearchContentRequest input)
    {
        throw new NotImplementedException();
    }
}
