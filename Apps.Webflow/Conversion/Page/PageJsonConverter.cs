using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Apps.Webflow.Conversion.Page;

public static class PageJsonConverter
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static Stream ToJson(PageDomEntity page, string siteId, string? localeId)
    {
        var model = new DownloadedPage
        {
            Page = page,
            SiteId = siteId,
            LocaleId = localeId,
        };

        var jsonString = JsonConvert.SerializeObject(model, settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
