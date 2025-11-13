using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Entities.CollectionItem;
using Newtonsoft.Json;
using System.Text;

namespace Apps.Webflow.Conversion.CollectionItem;

public static class CollectionItemJsonConverter
{
    public static Stream ToJson(
        CollectionItemEntity item,
        string collectionId,
        string siteId,
        string? locale)
    {
        var model = new DownloadedCollectionItem
        {
            CollectionId = collectionId,
            SiteId = siteId,
            CollectionItem = item,
            Locale = locale
        };

        var jsonString = JsonConvert.SerializeObject(model, JsonConfig.Settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}