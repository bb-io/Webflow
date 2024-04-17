using Apps.Webflow.Models.Response;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Webflow.Api;

public class WebflowClient : BlackBirdRestClient
{
    private const int Limit = 100;

    public WebflowClient() : base(new()
    {
        BaseUrl = "https://api.webflow.com/v2".ToUri()
    })
    {
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<WebflowError>(response.Content!)!;
        return new($"{error.Code}: {error.Message}");
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
}