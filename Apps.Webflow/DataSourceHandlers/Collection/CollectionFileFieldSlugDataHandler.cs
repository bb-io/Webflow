using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities.Collection;
using Apps.Webflow.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class CollectionFileFieldSlugDataHandler : WebflowInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _collectionId;
    
    public CollectionFileFieldSlugDataHandler(
        InvocationContext context, 
        [ActionParameter] CollectionIdentifier collectionIdentifier) : base(context)
    {
        if (string.IsNullOrWhiteSpace(collectionIdentifier.CollectionId))
            throw new PluginMisconfigurationException("Please specify a collection ID first");

        _collectionId = collectionIdentifier.CollectionId;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"collections/{_collectionId}");
        var collection = await Client.ExecuteWithErrorHandling<CollectionEntity>(request);

        var fields = collection.Fields.ToList();
        return fields
            .Where(x => x.Type == "File")
            .Select(x => new DataSourceItem(x.Slug, $"{x.DisplayName} ({x.Slug})"))
            .Where(x => 
                string.IsNullOrEmpty(context.SearchString) || 
                x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}