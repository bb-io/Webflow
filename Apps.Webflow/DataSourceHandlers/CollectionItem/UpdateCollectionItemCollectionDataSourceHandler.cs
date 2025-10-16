using Apps.Webflow.Api;
using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Collection;
using Apps.Webflow.Models.Request.CollectionItem;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem
{
    public class UpdateCollectionItemCollectionDataSourceHandler : BaseCollectionDataSourceHandler
    {
        public UpdateCollectionItemCollectionDataSourceHandler(InvocationContext invocationContext,
          [ActionParameter] UpdateCollectionItemRequest request)
          : base(invocationContext, request.SiteId)
        {
        }
    }
}
