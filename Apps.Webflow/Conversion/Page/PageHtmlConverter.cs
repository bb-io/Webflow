using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Response.Pages;
using HtmlAgilityPack;
using System.Web;

namespace Apps.Webflow.Conversion.Page;

public static class PageHtmlConverter
{
    private static readonly string[] TranslatableNodeTypes = { "text", "component-instance" };
    private static readonly string[] TranslatablePropertyTypes = { "Plain Text", "Rich Text" };

    public static Stream ToHtml(PageDomEntity pageDom, string siteId, string pageId, string? pageTitle, string? localeId)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head != null)
        {
            if (!string.IsNullOrEmpty(pageTitle))
            {
                var titleNode = head.SelectSingleNode("title");
                if (titleNode == null)
                {
                    titleNode = doc.CreateElement("title");
                    head.PrependChild(titleNode);
                }

                titleNode.InnerHtml = pageTitle;
            }

            var mType = doc.CreateElement("meta");
            mType.SetAttributeValue("name", "blackbird-content-type");
            mType.SetAttributeValue("content", ContentTypes.Page.ToKebabCase());
            head.AppendChild(mType);

            var metaSiteId = doc.CreateElement("meta");
            metaSiteId.SetAttributeValue("name", "blackbird-site-id");
            metaSiteId.SetAttributeValue("content", siteId);
            head.AppendChild(metaSiteId);

            var metaPageId = doc.CreateElement("meta");
            metaPageId.SetAttributeValue("name", "blackbird-page-id");
            metaPageId.SetAttributeValue("content", pageId);
            head.AppendChild(metaPageId);

            if (localeId != null)
            {
                var metaLocaleId = doc.CreateElement("meta");
                metaLocaleId.SetAttributeValue("name", "blackbird-locale-id");
                metaLocaleId.SetAttributeValue("content", localeId);
                head.AppendChild(metaLocaleId);
            }
        }


        foreach (var node in pageDom.Nodes)
        {
            if (node.Type == "image")
                continue;

            if (!TranslatableNodeTypes.Contains(node.Type))
                continue;

            if (node.Type == "text")
            {
                var textHtml = node.Text?.Html??node.Text?.Text ?? string.Empty;

                var divNode = doc.CreateElement("div");
                divNode.SetAttributeValue(ConversionConstants.NodeId, node.Id);
                divNode.InnerHtml = textHtml;
                body.AppendChild(divNode);
            }
            else if (node.Type == "component-instance")
            {
                foreach (var prop in node.PropertyOverrides)
                {
                    if (TranslatablePropertyTypes.Contains(prop.Type))
                    {
                        var textHtml = prop.Text?.Html ?? prop.Text?.Text ?? string.Empty;

                        var divNode = doc.CreateElement("div");
                        divNode.SetAttributeValue(ConversionConstants.NodeId, node.Id);
                        divNode.SetAttributeValue(ConversionConstants.PropertyId, prop.PropertyId);
                        divNode.InnerHtml = textHtml;

                        body.AppendChild(divNode);
                    }
                }
            }
        }

        var result = new MemoryStream();
        doc.Save(result);
        result.Position = 0;
        return result;
    }

    public static PageDomEntity ToJson(Stream fileStream, PageDomEntity originalPageDom)
    {
        fileStream.Position = 0;
        var doc = new HtmlDocument();
        doc.Load(fileStream);

        var elements = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.NodeId] != null)
            .ToList();

        foreach (var element in elements)
        {
            var nodeId = element.Attributes[ConversionConstants.NodeId].Value;
            var node = originalPageDom.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null)
                continue;

            if (node.Type == "text")
            {
                node.Text.Html = HttpUtility.HtmlDecode(element.InnerHtml);
            }
            else if (node.Type == "component-instance")
            {
                var propertyId = element.Attributes[ConversionConstants.PropertyId]?.Value;
                if (!string.IsNullOrEmpty(propertyId))
                {
                    var prop = node.PropertyOverrides
                        .FirstOrDefault(p => p.PropertyId == propertyId);

                    if (prop != null && TranslatablePropertyTypes.Contains(prop.Type))
                    {
                        prop.Text.Html = HttpUtility.HtmlDecode(element.InnerHtml);
                    }
                }
            }
        }

        return originalPageDom;
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
