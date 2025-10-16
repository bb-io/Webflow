using Apps.Webflow.Models.Request.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Locale
{
    public class UpdateCollectionItemLocaleDataSourceHandler : CmsLocaleDataSourceHandler
    {
        public UpdateCollectionItemLocaleDataSourceHandler(InvocationContext invocationContext,
            [ActionParameter] UpdateCollectionItemRequest request) : base(invocationContext, request.SiteId)
        {
        }
    }
}