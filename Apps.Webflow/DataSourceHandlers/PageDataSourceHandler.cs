using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers
{
    public class PageDataSourceHandler : WebflowInvocable, IAsyncDataSourceItemHandler
    {
        private readonly GetPageAsHtmlRequest _input;

        public PageDataSourceHandler(InvocationContext invocationContext, [ActionParameter] GetPageAsHtmlRequest input):base(invocationContext)
        {
            _input = input;
        }

        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var siteId = _input.SiteId;

            var endpoint = $"sites/{siteId}/pages";
            var request = new RestRequest(endpoint, Method.Get);

            var response = await Client.ExecuteWithErrorHandling<ListPagesResponse>(request);

            var dataSourceItems = response.Pages
                .Select(page => new DataSourceItem
                {
                    Value = page.Id,
                    DisplayName = $"{page.Title}"
                })
                .ToList();

            return dataSourceItems;
        }
    }
}
