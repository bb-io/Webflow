using Apps.Webflow.Api;
using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Response.Components;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Webflow.Conversion.Component;

public static class ComponentHtmlConverter
{
    private static readonly HashSet<string> TranslatablePropertyTypes = ["Plain Text", "Rich Text"];
    private static readonly HashSet<string> NodeTypesWithText = 
        ["text", "text-input", "select", "submit-button", "component-instance"];

    public static async Task<Stream> ToHtml(
        ComponentDomEntity componentDom, 
        string siteId, 
        string componentId, 
        string? localeId,
        WebflowClient client)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head != null)
        {
            var mType = doc.CreateElement("meta");
            mType.SetAttributeValue("name", "blackbird-content-type");
            mType.SetAttributeValue("content", ContentTypes.Component.ToKebabCase());
            head.AppendChild(mType);

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
                metaLocaleId.SetAttributeValue("name", "blackbird-locale");
                metaLocaleId.SetAttributeValue("content", localeId);
                head.AppendChild(metaLocaleId);
            }
        }

        // Add component properties
        var properties = await FetchComponentProperties(client, siteId, componentId, localeId);
        foreach (var property in properties.Where(p => TranslatablePropertyTypes.Contains(p.Type)))
        {
            var div = GetDivFromComponentProperty(doc, property);
            body.AppendChild(div);
        }

        // Add nodes
        foreach (var node in componentDom.Nodes)
        {
            if (node.Type == "image")
                continue;

            if (!NodeTypesWithText.Contains(node.Type))
                continue;

            if (node.Type == "text" && node.Text is not null)
            {
                var div = GetDivFromComponentTextNode(doc, node.Text, node.Id);
                body.AppendChild(div);
            }
            else if (node.Type == "text-input" && !string.IsNullOrEmpty(node.Placeholder))
            {
                var div = GetDivFromNodeAttribute(doc, node.Id, "placeholder", node.Placeholder);
                body.AppendChild(div);
            }
            else if (node.Type == "select" && node.Choices is not null && node.Choices.Any())
            {
                foreach (var choice in node.Choices)
                {
                    var div = GetDivFromSelectChoice(doc, node.Id, choice);
                    body.AppendChild(div);
                }
            }
            else if (node.Type == "submit-button")
            {
                if (!string.IsNullOrEmpty(node.Value))
                {
                    var divValue = GetDivFromNodeAttribute(doc, node.Id, "value", node.Value);
                    body.AppendChild(divValue);
                }
                if (!string.IsNullOrEmpty(node.WaitingText))
                {
                    var divWaiting = GetDivFromNodeAttribute(doc, node.Id, "waiting-text", node.WaitingText);
                    body.AppendChild(divWaiting);
                }
            }
            else if (node.Type == "component-instance")
            {
                // Add text if present
                if (node.Text is not null)
                {
                    var div = GetDivFromComponentTextNode(doc, node.Text, node.Id);
                    body.AppendChild(div);
                }

                // Add property overrides
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

    private static async Task<List<ComponentPropertyEntity>> FetchComponentProperties(
        WebflowClient client,
        string siteId,
        string componentId,
        string? localeId)
    {
        var endpoint = $"sites/{siteId}/components/{componentId}/properties";
        var request = new RestRequest(endpoint, Method.Get);
        
        if (!string.IsNullOrEmpty(localeId))
        {
            request.AddQueryParameter("localeId", localeId);
        }

        var response = await client.ExecuteWithErrorHandling<ComponentPropertiesResponse>(request);
        return response.Properties;
    }

    private static HtmlNode GetDivFromComponentProperty(
        HtmlDocument doc,
        ComponentPropertyEntity property)
    {
        var textHtml = property.Text.Html ?? property.Text.Text ?? string.Empty;

        var divNode = doc.CreateElement("div");
        divNode.SetAttributeValue(ConversionConstants.PropertyIdAttr, property.PropertyId);
        divNode.InnerHtml = textHtml;

        return divNode;
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

    private static HtmlNode GetDivFromNodeAttribute(
        HtmlDocument doc,
        string nodeId,
        string attributeName,
        string value)
    {
        var divNode = doc.CreateElement("div");
        divNode.SetAttributeValue(ConversionConstants.NodeId, nodeId);
        divNode.SetAttributeValue($"data-node-{attributeName}", "true");
        divNode.InnerHtml = value;

        return divNode;
    }

    private static HtmlNode GetDivFromSelectChoice(
        HtmlDocument doc,
        string nodeId,
        SelectChoice choice)
    {
        var divNode = doc.CreateElement("div");
        divNode.SetAttributeValue(ConversionConstants.NodeId, nodeId);
        divNode.SetAttributeValue(ConversionConstants.NodeChoices, choice.Value);
        divNode.InnerHtml = choice.Text;

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
