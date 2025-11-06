using System.Text;
using Newtonsoft.Json;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Conversion.CollectionItem;

public static class CollectionItemJsonConverter
{
    public static Stream ToJson(
        CollectionItemEntity item,
        string collectionItemId,
        string collectionId,
        string siteId,
        string? cmsLocaleId)
    {
        var model = new DownloadedCollectionItem
        {
            CmsLocaleId = cmsLocaleId,
            CollectionItemId = collectionItemId,
            CollectionId = collectionId,
            SiteId = siteId,
            CollectionItem = item
        };

        var jsonString = JsonConvert.SerializeObject(model, Formatting.Indented);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }
}