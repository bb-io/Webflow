using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Pages;
using Newtonsoft.Json;
using System.Text;

namespace Apps.Webflow.Conversion.Page;

public static class PageJsonConverter
{
    public static Stream ToJson(PageDomEntity page, string siteId, string? pageTitle, string? localeId)
    {
        var model = new DownloadedPage
        {
            Title = pageTitle,
            Page = page,
            SiteId = siteId,
            Locale = localeId,
        };

        var jsonString = JsonConvert.SerializeObject(model, JsonConfig.Settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
