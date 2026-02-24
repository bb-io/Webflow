using Apps.Webflow.Constants;
using Apps.Webflow.Models.Error;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Apps.Webflow.Api;

public class WebflowClient : BlackBirdRestClient
{
    private const int Limit = 100;
    private const int MaxRetries = 5;
    private readonly IEnumerable<AuthenticationCredentialsProvider> _creds;
    private string ConnectionType => _creds.First(x => x.KeyName == CredsNames.ConnectionType).Value;

    public WebflowClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = "https://api.webflow.com/v2".ToUri()
    })
    {
        _creds = creds;

        var accessTokenProvider = creds.FirstOrDefault(x => x.KeyName == CredsNames.AccessToken);
        if (accessTokenProvider != null && !string.IsNullOrEmpty(accessTokenProvider.Value))
        {
            this.AddDefaultHeader("Authorization", $"Bearer {creds.Get(CredsNames.AccessToken).Value}");
        }
    }

    public async Task ValidateConnection()
    {
        if (ConnectionType is ConnectionTypes.OAuth2)
        {
            var siteId = _creds.First(x => x.KeyName == CredsNames.SiteId).Value;
            if (string.IsNullOrWhiteSpace(siteId))
                throw new PluginMisconfigurationException("Please specify valid site ID value");

            await ExecuteWithErrorHandling(new RestRequest($"sites/{siteId}", Method.Get));
        } 
        else
        {
            await ExecuteWithErrorHandling(new RestRequest("sites", Method.Get));
        }
    }

    public string GetSiteId(string? siteIdFromInput)
    {
        if (ConnectionType != ConnectionTypes.OAuth2)
            return siteIdFromInput ?? throw new PluginMisconfigurationException("Please specify the site ID input");

        if (siteIdFromInput != null)
            return siteIdFromInput;

        var siteId = _creds.FirstOrDefault(x => x.KeyName == CredsNames.SiteId)?.Value;
        return siteId ?? throw new PluginMisconfigurationException("Site ID was not found in credentials for OAuth2 connection");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<WebflowError>(response.Content!)!;
        return new PluginApplicationException(error.ToString());
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        int retryCount = 0;
        while (true)
        {
            var response = await ExecuteAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests && retryCount < MaxRetries)
            {
                retryCount++;

                var retryAfterHeader = response.Headers?.FirstOrDefault(h =>
                    h.Name?.Equals("Retry-After", StringComparison.OrdinalIgnoreCase) == true);

                int delaySeconds = 60; 
                if (retryAfterHeader?.Value != null && int.TryParse(retryAfterHeader.Value.ToString(), out var parsedDelay))
                {
                    delaySeconds = parsedDelay;
                }

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                continue;
            }

            throw ConfigureErrorException(response);
        }
    }

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        var response = await ExecuteWithErrorHandling(request);
        var content = response.Content;
        var result = JsonConvert.DeserializeObject<T>(content, JsonSettings);

        if (result == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return result;
    }

    public async Task<List<T>> Paginate<T>(RestRequest request)
    {
        var offset = 0;
        var baseUrl = request.Resource;

        var result = new List<T>();
        PaginationResponse<T> response;
        do
        {
            request.Resource = baseUrl
                .SetQueryParameter("offset", offset.ToString())
                .SetQueryParameter("limit", Limit.ToString());

            response = await ExecuteWithErrorHandling<PaginationResponse<T>>(request);
            result.AddRange(response.Items);

            offset += Limit;
        } while (result.Count < response.Pagination.Total);

        return result;
    }

    public async Task<List<T>> Paginate<T, TResponse>(RestRequest request, Func<TResponse, List<T>> itemsSelector)
        where TResponse : class, IPaginatableResponse<T>
    {
        var offset = 0;
        var baseUrl = request.Resource;

        var result = new List<T>();
        TResponse response;
        do
        {
            request.Resource = baseUrl
                .SetQueryParameter("offset", offset.ToString())
                .SetQueryParameter("limit", Limit.ToString());

            response = await ExecuteWithErrorHandling<TResponse>(request);
            var items = itemsSelector(response);
            result.AddRange(items);

            offset += Limit;
        } while (result.Count < response.Pagination.Total);

        return result;
    }
}