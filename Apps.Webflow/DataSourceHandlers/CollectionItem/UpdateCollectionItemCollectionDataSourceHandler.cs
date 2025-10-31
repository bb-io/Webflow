using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem;

public class UpdateCollectionItemCollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest site) 
    : BaseCollectionDataSourceHandler(invocationContext, site.SiteId)
{
}
