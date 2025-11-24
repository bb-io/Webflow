namespace Apps.Webflow.Models.Response.Pages;

public class PageDomEntity
{
    public string PageId { get; set; }
    public List<PageNodeEntity> Nodes { get; set; } = new List<PageNodeEntity>();
}
public class PageNodeEntity
{
    public string Id { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
    public PageTextContent Text { get; set; }
    public string Placeholder { get; set; }
    public string ComponentId { get; set; }
    public List<PropertyOverrideEntity>? PropertyOverrides { get; set; }
}
public class PageTextContent
{
    public string Html { get; set; }
    public string Text { get; set; }
}
public class PropertyOverrideEntity
{
    public string PropertyId { get; set; }
    public string Type { get; set; }
    public string Label { get; set; }
    public PageTextContent Text { get; set; }
}
