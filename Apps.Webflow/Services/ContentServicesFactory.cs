using Apps.Webflow.Constants;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services;

public class ContentServicesFactory(InvocationContext invocationContext)
{
    public IContentService<T> Create<T>(string contentType) where T : class, IDateFilterable
    {
        return contentType switch
        {
            ContentTypes.Page when typeof(T) == typeof(PageEntity) =>
                (IContentService<T>)new PageService(invocationContext),

            ContentTypes.Component when typeof(T) == typeof(ComponentResponse) =>
                (IContentService<T>)new ComponentService(invocationContext),

            _ => throw new PluginApplicationException($"This content type is not supported: {contentType}")
        };
    }
}
