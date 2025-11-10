using Apps.Webflow.Models.Response.Pages;

namespace Apps.Webflow.Conversion.Models;

public class DownloadedPage
{
    public string? Locale { get; set; }
    public required string SiteId { get; set; }
    public required PageDomEntity Page { get; set; }
}
