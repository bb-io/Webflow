using HtmlAgilityPack;

namespace Apps.Webflow.Extensions;

public static class HtmlExtensions
{
    public static string? GetMetaValue(this HtmlNode node, string metaName)
    {
        return node.SelectSingleNode($"//meta[@name='{metaName}']")?.GetAttributeValue("content", string.Empty);
    }

    public static string? GetDivText(this HtmlNode node, string id)
    {
        return node?.SelectSingleNode($"descendant::div[@id='{id}']")?.InnerHtml?.Trim();
    }

    public static bool? GetAttributeBool(this HtmlNode node, string id, string attribute)
    {
        return node?.SelectSingleNode($"descendant::div[@id='{id}']")?.GetAttributeValue(attribute, "false") == "true";
    }
}
