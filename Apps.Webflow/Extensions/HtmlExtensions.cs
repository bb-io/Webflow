namespace Apps.Webflow.Extensions;

public static class HtmlExtensions
{
    public static string? GetMetaValue(this HtmlAgilityPack.HtmlNode node, string metaName)
    {
        return node.SelectSingleNode($"//meta[@name='{metaName}']")?.GetAttributeValue("content", string.Empty);
    }
}
