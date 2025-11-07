using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Components;

namespace Apps.Webflow.Conversion.Component;

public class ComponentJsonConverter
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static Stream ToJson(
        ComponentDomEntity component,
        string siteId,
        string? localeId)
    {
        var model = new DownloadedComponent
        {
            Component = component,
            SiteId = siteId,
            LocaleId = localeId
        };

        var jsonString = JsonConvert.SerializeObject(model, settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
