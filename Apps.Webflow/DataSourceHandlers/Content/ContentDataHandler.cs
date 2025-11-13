using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Content;

public class ContentDataHandler(InvocationContext invocationContext, 
    [ActionParameter] ContentFilter contentFilter,
    [ActionParameter] SiteIdentifier site,
    [ActionParameter] string? CollectionId) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    private readonly ContentServicesFactory _factory = new(invocationContext, null!);

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(contentFilter.ContentType))
            throw new PluginMisconfigurationException("Please specify the 'Content type' input");

        if (string.IsNullOrEmpty(Client.GetSiteId(site.SiteId)))
            throw new PluginMisconfigurationException("Please specify the 'Site ID' input");

        var service = _factory.GetContentService(contentFilter.ContentType);
        var input = new SearchContentRequest { ContentTypes = [contentFilter.ContentType], CollectionIds = [CollectionId] };
        var dateFilter = new ContentDateFilter { };

        var result = await service.SearchContent(Client.GetSiteId(site.SiteId), input, dateFilter);
        return result.Items.Select(x => new DataSourceItem(x.ContentId, x.Name));
    }
}
