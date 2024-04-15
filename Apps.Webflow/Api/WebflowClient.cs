using Apps.Webflow.Models.Response;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Webflow.Api;

public class WebflowClient : BlackBirdRestClient
{
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
}