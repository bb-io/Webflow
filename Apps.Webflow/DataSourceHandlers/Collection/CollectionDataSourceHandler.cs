using Apps.Webflow.Models.Request.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionDataSourceHandler : BaseCollectionDataSourceHandler
{
    public CollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] CollectionRequest request)
        : base(invocationContext, request.SiteId)
    {
    }
}