using Apps.Webflow.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Response.Components;

namespace Apps.Webflow.Conversion.Models;

public class DownloadedComponent
{
    public string contentType = ContentTypes.Component.ToKebabCase();

    public string? Locale { get; set; }
    public required string SiteId { get; set; }
    public required ComponentDomEntity Component { get; set; }
    public List<ComponentPropertyEntity> Properties { get; set; } = new();
}
