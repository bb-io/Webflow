namespace Apps.Webflow.Constants;

public class ContentTypes
{
    public const string Page = "Page";
    public const string Component = "Component";
    public const string CollectionItem = "Collection item";

    public static readonly IEnumerable<string> SupportedContentTypes = [Page, Component, CollectionItem];
}
