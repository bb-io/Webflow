using RestSharp;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem;

public class CollectionItemDataSourceHandler(InvocationContext invocationContext, string siteId, string collectionId, string? cmsLocale) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(collectionId))
            throw new PluginMisconfigurationException("You need to specify Collection ID first");

        var request = new RestRequest($"collections/{collectionId}/items", Method.Get);
        if (!string.IsNullOrEmpty(cmsLocale))
        {
            var cmsLocaleId = await LocaleHelper.GetCmsLocaleId(cmsLocale, siteId, Client);
            request.AddQueryParameter("cmsLocaleId", cmsLocaleId);
        }

        var response = await Client.Paginate<CollectionItemEntity>(request);
        return response.Select(ci => new DataSourceItem(ci.Id, ci.Name));
    }
}