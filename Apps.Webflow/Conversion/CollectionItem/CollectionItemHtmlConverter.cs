using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Entities.Collection;
using Apps.Webflow.Models.Entities.CollectionItem;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Apps.Webflow.Conversion.CollectionItem;

public static class CollectionItemHtmlConverter
{
    private static readonly string[] UntraslatableSlugs = ["slug"];
    private static readonly string[] TranslatableTypes = ["RichText", "PlainText", "Link"];
    private static readonly string[] NonUpdatableTypes = ["Reference"];
    private static readonly char[] InvisibleChars = [' ', '\t', '\n', '\r', '\u200B', '\u200C', '\u200D', '\uFEFF'];

    public static Stream ToHtml(CollectionItemEntity item, IEnumerable<FieldEntity> collectionFields, string siteId,
        string collectionId, string itemId, string? slug, string? cmsLocaleId = null)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head != null)
        {
            AppendMeta(head, "blackbird-content-type", ContentTypes.CollectionItem.ToKebabCase());
            AppendMeta(head, "blackbird-site-id", siteId);
            AppendMeta(head, "blackbird-collection-id", collectionId);
            AppendMeta(head, "blackbird-collection-item-id", itemId);

            if (!string.IsNullOrEmpty(cmsLocaleId))
                AppendMeta(head, "blackbird-cmslocale", cmsLocaleId);

            if (!string.IsNullOrEmpty(slug))
                AppendMeta(head, "blackbird-collection-item-slug", slug);
        }

        var translatableFields = collectionFields
            .Where(x => TranslatableTypes.Contains(x.Type) && !UntraslatableSlugs.Contains(x.Slug))
            .Select(x => x.Slug)
            .ToArray();

        foreach (var x in item.FieldData.Children()
                     .OfType<JProperty>()
                     .Where(f => translatableFields.Contains(f.Name)))
        {
            var rawHtml = x.Value.ToString();
            var cleanedHtml = CleanHtmlFragment(rawHtml);

            if (!string.IsNullOrWhiteSpace(cleanedHtml))
            {
                var node = doc.CreateElement(HtmlConstants.Div);
                node.InnerHtml = cleanedHtml;
                node.SetAttributeValue(ConversionConstants.FieldSlug, x.Name);
                body.AppendChild(node);
            }
        }

        var result = new MemoryStream();
        doc.Save(result);
        result.Position = 0;

        return result;
    }

    private static string CleanHtmlFragment(string html)
    {
        var fragDoc = new HtmlDocument();
        fragDoc.LoadHtml(html);

        RemoveEmptyNodes(fragDoc.DocumentNode);

        return fragDoc.DocumentNode.InnerHtml;
    }

    private static void RemoveEmptyNodes(HtmlNode node)
    {
        foreach (var child in node.ChildNodes.ToList())
        {
            RemoveEmptyNodes(child);

            if (child.Name.Equals("br", StringComparison.OrdinalIgnoreCase))
                continue;

            var text = HtmlEntity.DeEntitize(child.InnerText)?.Trim(InvisibleChars);

            if (child.NodeType == HtmlNodeType.Element &&
                string.IsNullOrEmpty(text) &&
                !child.ChildNodes.Any(c => c.NodeType == HtmlNodeType.Element))
            {
                child.Remove();
            }
        }
    }

    private static void AppendMeta(HtmlNode head, string name, string content)
    {
        var meta = head.OwnerDocument.CreateElement("meta");
        meta.SetAttributeValue("name", name);
        meta.SetAttributeValue("content", content);
        head.AppendChild(meta);
    }

    public static JObject ToJson(Stream fileStream, JObject fieldData, IEnumerable<FieldEntity> collectionFields)
    {
        var doc = new HtmlDocument();
        doc.Load(fileStream);

        doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType is HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.FieldSlug]?.Value != null)
            .ToList()
            .ForEach(x =>
            {
                var slug = x.Attributes[ConversionConstants.FieldSlug].Value;
                fieldData[slug] = HttpUtility.HtmlDecode(x.InnerHtml);
            });

        fieldData
            .Children()
            .OfType<JProperty>()
            .Where(prop => NonUpdatableTypes.Contains(collectionFields.FirstOrDefault(x => x.Slug == prop.Name)?.Type))
            .ToList()
            .ForEach(x => x.Remove());

        return fieldData;
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument()
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement(HtmlConstants.Head);

        headNode.AppendChild(htmlDoc.CreateElement(HtmlConstants.Title));
        htmlNode.AppendChild(headNode);

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }
}