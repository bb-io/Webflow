using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.DataSourceHandlers.Content;

public class ContentDataHandler(InvocationContext invocationContext, 
    [ActionParameter] ContentFilter contentFilter,
    [ActionParameter] SiteRequest site,
    [ActionParameter] string? CollectionId) 
    : WebflowInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(contentFilter.ContentType))
            throw new PluginMisconfigurationException("Please specify the 'Content type' input");

        if (string.IsNullOrEmpty(site.SiteId))
            throw new PluginMisconfigurationException("Please specify the 'Site ID' input");

        var service = _factory.GetContentService(contentFilter.ContentType);
        var input = new SearchContentRequest { ContentTypes = [contentFilter.ContentType], CollectionId = CollectionId };
        var dateFilter = new DateFilter { };

        var result = await service.SearchContent(site, input, dateFilter);
        return result.Items.Select(x => new DataSourceItem(x.ContentId, x.Name));
    }
}
