﻿using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers.Collection;

public class UpdateCollectionItemDataSourceHandler(InvocationContext invocationContext,
    [ActionParameter] UpdateCollectionItemRequest request) : WebflowInvocable(invocationContext), IAsyncDataSourceHandler
{
    private UpdateCollectionItemRequest Request { get; } = request;

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Request.CollectionId))
            throw new("You need to specify Collection ID first");

        var endpoint = $"collections/{Request.CollectionId}/items";

        endpoint = string.IsNullOrEmpty(Request.CmsLocaleId)
            ? endpoint
            : endpoint.SetQueryParameter("cmsLocaleIds", Request.CmsLocaleId);
        var request = new RestRequest(endpoint, Method.Get);
        var response = await Client.Paginate<CollectionItemEntity>(request);

        return response
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.LastUpdated)
            .Take(50)
            .ToDictionary(x => x.Id, x => x.Name);
    }
}