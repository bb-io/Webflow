using Apps.Webflow.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Models.Response.Pages;

namespace Apps.Webflow.Conversion.Models;

public class DownloadedPage
{
    public string contentType = ContentTypes.Page.ToKebabCase();

    public string? Title { get; set; }
    public string? Locale { get; set; }
    public required string SiteId { get; set; }
    public required PageDomEntity Page { get; set; }
}
