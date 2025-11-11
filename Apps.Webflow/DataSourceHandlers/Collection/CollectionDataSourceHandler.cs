using Apps.Webflow.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteIdentifier request)
    : BaseCollectionDataSourceHandler(invocationContext, request.SiteId)
{
}