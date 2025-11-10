using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Extensions;
using Apps.Webflow.Services;
using Newtonsoft.Json;
using System.Text;

namespace Apps.Webflow.Conversion.CollectionItem;

public static class CollectionItemMetadataParser
{
    public static async Task<ParsedContentMetadata> Parse(Stream stream)
    {
        if (stream.CanSeek) stream.Position = 0;

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        var content = await reader.ReadToEndAsync();

        if (stream.CanSeek) stream.Position = 0;

        if (JsonHelper.IsJson(content))
        {
            var item = JsonConvert.DeserializeObject<DownloadedCollectionItem>(content);
            return new ParsedContentMetadata
            {
                CollectionId = item?.CollectionId,
                CollectionItemId = item?.CollectionItem?.Id,
                Locale = item?.Locale
            };
        }

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(content);

        return new ParsedContentMetadata
        {
            CollectionId = doc.DocumentNode.GetMetaValue("blackbird-collection-id"),
            CollectionItemId = doc.DocumentNode.GetMetaValue("blackbird-collection-item-id"),
            Locale = doc.DocumentNode.GetMetaValue("blackbird-cmslocale")
        };
    }
}
