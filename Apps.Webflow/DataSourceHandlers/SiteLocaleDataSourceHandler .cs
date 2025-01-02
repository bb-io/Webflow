using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Webflow.Api;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Pages;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Webflow.DataSourceHandlers
{
    public class SiteLocaleDataSourceHandler : WebflowInvocable, IAsyncDataSourceItemHandler
    {
        private readonly UpdatePageContentRequest _input;

        public SiteLocaleDataSourceHandler(InvocationContext invocationContext, UpdatePageContentRequest input) : base(invocationContext)
        {
            _input = input; 
        }

        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_input.SiteId))
                throw new Exception("Site ID cannot be null or empty.");

            var endpoint = $"sites/{_input.SiteId}";
            var request = new WebflowRequest(endpoint, Method.Get, Creds);

            var siteResponse = await Client.ExecuteWithErrorHandling<SiteLocales>(request);

            if (siteResponse.Locales == null)
                return Enumerable.Empty<DataSourceItem>();

            var localeItems = siteResponse.Locales.Secondary
                .Append(siteResponse.Locales.Primary)
                .Where(locale => locale != null)
                .Select(locale => new DataSourceItem
                {
                    Value = locale.Id,
                    DisplayName = locale.DisplayName
                });

            return localeItems;
        }
    }
}
