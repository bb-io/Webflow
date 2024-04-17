using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionItemCollectionDataSourceHandler : BaseCollectionDataSourceHandler
{
    public CollectionItemCollectionDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] CollectionItemRequest request)
        : base(invocationContext, request.SiteId)
    {
    }
}