using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Models.Response.Components;
using HtmlAgilityPack;

namespace Apps.Webflow.HtmlConversion;

public static class ComponentHtmlConverter
{
    private static readonly HashSet<string> TranslatableNodeTypes = ["text", "component-instance"];
    private static readonly HashSet<string> TranslatablePropertyTypes = ["Plain Text", "Rich Text"];

    public static Stream ToHtml(ComponentDomEntity componentDom, string siteId, string componentId, string? localeId)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head != null)
        {
            var metaSiteId = doc.CreateElement("meta");
            metaSiteId.SetAttributeValue("name", "blackbird-site-id");
            metaSiteId.SetAttributeValue("content", siteId);
            head.AppendChild(metaSiteId);

            var metaComponentId = doc.CreateElement("meta");
            metaComponentId.SetAttributeValue("name", "blackbird-component-id");
            metaComponentId.SetAttributeValue("content", componentId);
            head.AppendChild(metaComponentId);

            if (localeId != null)
            {
                var metaLocaleId = doc.CreateElement("meta");
                metaLocaleId.SetAttributeValue("name", "blackbird-locale-id");
                metaLocaleId.SetAttributeValue("content", localeId);
                head.AppendChild(metaLocaleId);
            }
        }

        foreach (var node in componentDom.Nodes)
        {
            if (node.Type == "image")
                continue;

            if (!TranslatableNodeTypes.Contains(node.Type))
                continue;

            if (node.Type == "text" && node.Text is not null)
            {
                var div = GetDivFromComponentTextNode(doc, node.Text, node.Id);
                body.AppendChild(div);
            }
            else if (node.Type == "component-instance")
            {
                if (node.PropertyOverrides is null)
                    continue;

                foreach (var prop in node.PropertyOverrides.Where(p => TranslatablePropertyTypes.Contains(p.Type)))
                {
                    var div = GetDivFromComponentTextNode(doc, prop.Text, node.Id, prop.PropertyId);
                    body.AppendChild(div);
                }
            }
        }

        var result = new MemoryStream();
        doc.Save(result);
        result.Position = 0;
        return result;
    }

    private static HtmlNode GetDivFromComponentTextNode(
        HtmlDocument doc,
        ComponentTextContent textComponent,
        string nodeId,
        string? propertyId = null)
    {
        var textHtml = textComponent.Html ?? textComponent.Text ?? string.Empty;

        var divNode = doc.CreateElement("div");
        divNode.SetAttributeValue(ConversionConstants.NodeId, nodeId);

        if (propertyId is not null)
        {
            divNode.SetAttributeValue(ConversionConstants.PropertyId, propertyId);
        }
        
        divNode.InnerHtml = textHtml;

        return divNode;
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument()
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement("head");
        headNode.AppendChild(htmlDoc.CreateElement("title"));
        htmlNode.AppendChild(headNode);

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }
}
