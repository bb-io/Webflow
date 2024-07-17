using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class PublishItemRequestCollectionDataSourceHandler : BaseCollectionDataSourceHandler
{
    public PublishItemRequestCollectionDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] PublishItemRequest request)
        : base(invocationContext, request.SiteId)
    {
    }
}