using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Components;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services.Concrete;

public class ComponentService(InvocationContext invocationContext) : BaseContentService<ComponentResponse>(invocationContext)
{
    public override Task<IEnumerable<ComponentResponse>> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter)
    {
        throw new NotImplementedException();
    }
}
