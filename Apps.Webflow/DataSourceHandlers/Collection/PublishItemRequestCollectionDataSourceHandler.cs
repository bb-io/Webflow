using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class PublishItemRequestCollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest request) 
    : BaseCollectionDataSourceHandler(invocationContext, request.SiteId)
{
}