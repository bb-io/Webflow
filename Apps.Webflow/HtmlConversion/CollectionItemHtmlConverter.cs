using System.Web;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Models.Entities;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Webflow.HtmlConversion;

public static class CollectionItemHtmlConverter
{
    private static readonly string[] TranslatableTypes = ["RichText", "PlainText", "Link"];

    public static Stream ToHtml(CollectionItemEntity item, IEnumerable<FieldEntity> collectionFields)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        var translatableFields = collectionFields
            .Where(x => TranslatableTypes.Contains(x.Type))
            .Select(x => x.Slug)
            .ToArray();

        item.FieldData.Children()
            .Where(x => x is JProperty jProperty && translatableFields.Contains(jProperty.Name))
            .Cast<JProperty>()
            .ToList()
            .ForEach(x =>
            {
                var node = doc.CreateElement(HtmlConstants.Div);
                node.InnerHtml = x.Value.ToString();
                node.SetAttributeValue(ConversionConstants.FieldSlug, x.Name);

                body.AppendChild(node);
            });

        var result = new MemoryStream();
        doc.Save(result);
        result.Position = 0;

        return result;
    }

    public static JObject ToJson(Stream fileStream, JObject fieldData)
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