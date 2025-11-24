using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Response.Pages;
using HtmlAgilityPack;
using System.Web;

namespace Apps.Webflow.Conversion.Page;

public static class PageHtmlConverter
{
    private static readonly string[] TranslatableNodeTypes = { "text", "text-input", "component-instance" };
    private static readonly string[] TranslatablePropertyTypes = { "Plain Text", "Rich Text" };

    public static Stream ToHtml(PageDomEntity pageDom, string siteId, string pageId, string? locale, PageMetadata? metadata)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head != null)
        {
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

            if (locale != null)
            {
                var metaLocaleId = doc.CreateElement("meta");
                metaLocaleId.SetAttributeValue("name", "blackbird-locale");
                metaLocaleId.SetAttributeValue("content", locale);
                head.AppendChild(metaLocaleId);
            }
        }

        if (metadata is not null)
            AddTranslatableMetadata(body, doc, metadata);

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
            else if (node.Type == "text-input" && !string.IsNullOrEmpty(node.Placeholder))
            {
                var divNode = doc.CreateElement("div");
                divNode.SetAttributeValue(ConversionConstants.NodeId, node.Id);
                divNode.SetAttributeValue("data-node-placeholder", "true");
                divNode.InnerHtml = node.Placeholder;
                body.AppendChild(divNode);
            }
            else if (node.Type == "component-instance")
            {
                if (node.PropertyOverrides is not null)
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
        }

        var result = new MemoryStream();
        doc.Save(result);
        result.Position = 0;
        return result;
    }

    private static void AddTranslatableMetadata(HtmlNode body, HtmlDocument doc, PageMetadata metadata)
    {
        if (metadata.PageTitle != null)
            AppendMetadataDiv(doc, body, "blackbird-page-title", metadata.PageTitle);

        if (metadata.Slug != null)
            AppendMetadataDiv(doc, body, "blackbird-page-slug", metadata.Slug);

        if (metadata.Seo != null)
        {
            AppendMetadataDiv(doc, body, "blackbird-seo-title", metadata.Seo.Title);
            AppendMetadataDiv(doc, body, "blackbird-seo-description", metadata.Seo.Description);
        }

        if (metadata.OpenGraph != null)
        {
            AppendMetadataDiv(
                doc, body,
                "blackbird-opengraph-title",
                metadata.OpenGraph.Title,
                "data-copied",
                (metadata.OpenGraph.TitleCopied == true).ToString().ToLower()
            );

            AppendMetadataDiv(
                doc, body,
                "blackbird-opengraph-description",
                metadata.OpenGraph.Description,
                "data-copied",
                (metadata.OpenGraph.DescriptionCopied == true).ToString().ToLower()
            );
        }
    }

    private static void AppendMetadataDiv(HtmlDocument doc, HtmlNode container, string id, string? value, string? dataAttributeName = null, string? dataAttributeValue = null)
    {
        var node = doc.CreateElement("div");
        node.SetAttributeValue("id", id);
        node.InnerHtml = value ?? "";

        if (dataAttributeName != null && dataAttributeValue != null)
            node.SetAttributeValue(dataAttributeName, dataAttributeValue);

        container.AppendChild(node);
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
