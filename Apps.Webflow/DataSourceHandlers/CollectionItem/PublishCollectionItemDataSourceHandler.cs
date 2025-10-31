using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem;

public class PublishCollectionItemDataSourceHandler(InvocationContext invocationContext, [ActionParameter] PublishItemRequest request) 
    : BaseCollectionItemDataSourceHandler(invocationContext, request.CollectionId, string.Empty)
{
}