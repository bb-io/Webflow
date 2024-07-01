using Apps.Webflow.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Locale;

public class SiteCmsLocaleDataSourceHandler : CmsLocaleDataSourceHandler
{
    public SiteCmsLocaleDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] SiteCmsLocaleRequest request) : base(invocationContext,
        request.SiteId)
    {
    }
}