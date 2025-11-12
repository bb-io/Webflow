using Apps.Webflow.Constants;
using Apps.Webflow.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Webflow.Services;

public class ContentServicesFactory(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
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
            ContentTypes.Page => new PageService(invocationContext, fileManagementClient),
            ContentTypes.Component => new ComponentService(invocationContext, fileManagementClient),
            ContentTypes.CollectionItem => new CollectionItemService(invocationContext, fileManagementClient),
            _ => throw new PluginApplicationException($"This content type is not supported: {contentType}")
        };
    }
}
