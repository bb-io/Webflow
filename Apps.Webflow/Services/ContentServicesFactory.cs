using Apps.Webflow.Constants;
using Apps.Webflow.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services;

public class ContentServicesFactory(InvocationContext invocationContext)
{
    public IContentService Create(string contentType)
    {
        return contentType switch
        {
            ContentTypes.Page => new PageService(invocationContext),
            ContentTypes.Component => new ComponentService(invocationContext),
            _ => throw new PluginApplicationException($"This content type is not supported: {contentType}")
        };
    }
}
