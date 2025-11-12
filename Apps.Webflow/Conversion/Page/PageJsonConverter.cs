using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Response.Pages;
using Newtonsoft.Json;
using System.Text;

namespace Apps.Webflow.Conversion.Page;

public static class PageJsonConverter
{
    public static Stream ToJson(PageDomEntity pageDom, string siteId, string? locale, PageMetadata metadata)
    {
        var model = new DownloadedPage
        {
            Page = pageDom,
            SiteId = siteId,
            Locale = locale,
            Metadata = metadata
        };

        var jsonString = JsonConvert.SerializeObject(model, JsonConfig.Settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}
