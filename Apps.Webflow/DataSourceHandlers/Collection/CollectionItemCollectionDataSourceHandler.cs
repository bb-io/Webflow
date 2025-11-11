using Apps.Webflow.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionItemCollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteIdentifier site) 
    : BaseCollectionDataSourceHandler(invocationContext, site.SiteId)
{
}