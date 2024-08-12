using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem;

public class PublishCollectionItemDataSourceHandler : BaseCollectionItemDataSourceHandler
{
    public PublishCollectionItemDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] PublishItemRequest request) : base(invocationContext, request.CollectionId, string.Empty)
    {
    }
}