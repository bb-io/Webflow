using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class CollectionItemLocaleDataSourceHandler : CmsLocaleDataSourceHandler
{
    public CollectionItemLocaleDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] CollectionItemRequest request) : base(invocationContext, request.SiteId)
    {
    }
}