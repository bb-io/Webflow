using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class CollectionItemLocaleDataSourceHandler : CmsLocaleDataSourceHandler
{
    public CollectionItemLocaleDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] SiteRequest request) : base(invocationContext, request.SiteId)
    {
    }
}