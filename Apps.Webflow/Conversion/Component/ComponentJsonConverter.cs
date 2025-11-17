using Apps.Webflow.Api;
using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Components;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Newtonsoft.Json;
using RestSharp;
using System.Text;

namespace Apps.Webflow.Conversion.Component;

public class ComponentJsonConverter
{
    public static async Task<Stream> ToJson(
        ComponentDomEntity component,
        string siteId,
        string? localeId,
        WebflowClient client)
    {
        // Fetch properties
        var endpoint = $"sites/{siteId}/components/{component.ComponentId}/properties";
        var request = new RestRequest(endpoint, Method.Get);
        
        if (!string.IsNullOrEmpty(localeId))
        {
            request.AddQueryParameter("localeId", localeId);
        }

        var propertiesResponse = await client.ExecuteWithErrorHandling<ComponentPropertiesResponse>(request);

        var model = new DownloadedComponent
        {
            Component = component,
            SiteId = siteId,
            Locale = localeId,
            Properties = propertiesResponse.Properties
        };

        var jsonString = JsonConvert.SerializeObject(model, JsonConfig.Settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
