using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class UpdateCollectionItemLocaleDataSourceHandler(InvocationContext invocationContext, [ActionParameter] SiteRequest site) 
    : CmsLocaleDataSourceHandler(invocationContext, site.SiteId)
{
}