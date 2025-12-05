using System.Text;
using Newtonsoft.Json;
using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Components;

namespace Apps.Webflow.Conversion.Component;

public class ComponentJsonConverter
{
    public static async Task<Stream> ToJson(
        ComponentDomEntity component,
        string siteId,
        string? localeId,
        List<ComponentPropertyEntity> properties)
    {
        var model = new DownloadedComponent
        {
            Component = component,
            SiteId = siteId,
            Locale = localeId,
            Properties = properties
        };

        var jsonString = JsonConvert.SerializeObject(model, JsonConfig.Settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
