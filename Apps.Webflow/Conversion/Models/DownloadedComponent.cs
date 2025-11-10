using Apps.Webflow.Models.Response.Components;

namespace Apps.Webflow.Conversion.Models;

public class DownloadedComponent
{
    public string? Locale { get; set; }
    public required string SiteId { get; set; }
    public required ComponentDomEntity Component { get; set; }
}
