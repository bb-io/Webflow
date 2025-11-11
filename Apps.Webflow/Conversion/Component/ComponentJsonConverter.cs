using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Components;
using Newtonsoft.Json;
using System.Text;

namespace Apps.Webflow.Conversion.Component;

public class ComponentJsonConverter
{
    public static Stream ToJson(
        ComponentDomEntity component,
        string siteId,
        string? localeId)
    {
        var model = new DownloadedComponent
        {
            Component = component,
            SiteId = siteId,
            Locale = localeId
        };

        var jsonString = JsonConvert.SerializeObject(model, JsonConfig.Settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
