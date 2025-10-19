using Apps.Webflow.Api;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Response.Components;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers
{
    public class ComponentDataSourceHandler : WebflowInvocable, IAsyncDataSourceItemHandler
    {
        private readonly GetComponentContentRequest _input;

        public ComponentDataSourceHandler(InvocationContext invocationContext, [ActionParameter] GetComponentContentRequest input) : base(invocationContext)
        {
            _input = input;
        }

        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var siteId = _input.SiteId;

            var endpoint = $"sites/{siteId}/components";
            var request = new WebflowRequest(endpoint, Method.Get, Creds);

            var response = await Client.ExecuteWithErrorHandling<ListComponentsResponse>(request);

            var dataSourceItems = response.Components?
                .Select(component => new DataSourceItem
                {
                    Value = component.Id,
                    DisplayName = component.Name
                })
                .ToList() ?? new List<DataSourceItem>();

            return dataSourceItems;
        }
    }
}
