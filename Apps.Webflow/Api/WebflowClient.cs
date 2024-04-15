using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Webflow.Api;

public class WebflowClient : BlackBirdRestClient
{
    public WebflowClient() : base(new())
    {
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        throw new NotImplementedException();
    }
}