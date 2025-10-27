using Apps.Webflow.Constants;
using Apps.Webflow.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services;

public class ContentServicesFactory(InvocationContext invocationContext)
{
    public List<IContentService> GetContentServices(IEnumerable<string> contentTypes)
    {
        var contentServices = new List<IContentService>();

        foreach (var contentType in contentTypes)
            contentServices.Add(GetContentService(contentType));

        return contentServices;
    }

    public IContentService GetContentService(string contentType)
    {
        return contentType switch
        {
            ContentTypes.Page => new PageService(invocationContext),
            ContentTypes.Component => new ComponentService(invocationContext),
            ContentTypes.CollectionItem => new CollectionItemService(invocationContext),
            _ => throw new PluginApplicationException($"This content type is not supported: {contentType}")
        };
    }
}
