using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest request) 
    : BaseCollectionDataSourceHandler(invocationContext, request.SiteId)
{
}