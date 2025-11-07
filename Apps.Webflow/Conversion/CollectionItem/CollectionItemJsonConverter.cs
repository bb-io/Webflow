using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Apps.Webflow.Conversion.CollectionItem;

public static class CollectionItemJsonConverter
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static Stream ToJson(
        CollectionItemEntity item,
        string collectionId,
        string siteId)
    {
        var model = new DownloadedCollectionItem
        {
            CollectionId = collectionId,
            SiteId = siteId,
            CollectionItem = item
        };

        var jsonString = JsonConvert.SerializeObject(model, settings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }

    public static CollectionItemMetadata GetMetadata(string inputString)
    {
        var json = JObject.Parse(inputString);

        return new CollectionItemMetadata(
            json["collectionId"]?.ToString(),
            json["collectionItemId"]?.ToString(),
            json["cmsLocaleId"]?.ToString()
        );
    }
}