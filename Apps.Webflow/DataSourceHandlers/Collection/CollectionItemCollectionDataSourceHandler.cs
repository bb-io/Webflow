using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionItemCollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest site) 
    : BaseCollectionDataSourceHandler(invocationContext, site.SiteId)
{
}