using RestSharp;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using Apps.Webflow.Models.Identifiers;

namespace Apps.Webflow.DataSourceHandlers.CollectionItem;

public class CollectionItemDataSourceHandler(
    InvocationContext invocationContext,
    [ActionParameter] SiteIdentifier site,
    [ActionParameter] CollectionIdentifier collection,
    [ActionParameter] LocaleIdentifier cmsLocale) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(collection.CollectionId))
            throw new PluginMisconfigurationException("You need to specify Collection ID first");

        var request = new RestRequest($"collections/{collection.CollectionId}/items", Method.Get);
        if (!string.IsNullOrEmpty(cmsLocale.Locale))
        {
            var cmsLocaleId = await LocaleHelper.GetCmsLocaleId(cmsLocale.Locale, Client.GetSiteId(site.SiteId), Client);
            request.AddQueryParameter("cmsLocaleId", cmsLocaleId);
        }

        var response = await Client.Paginate<CollectionItemEntity>(request);
        return response.Select(ci => new DataSourceItem(ci.Id, ci.Name));
    }
}