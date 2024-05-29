using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Response;

public class SitePublishedResponse
{
    [Display("Site ID")]
    public string SiteId { get; set; }
    
    [Display("Published on")]
    public DateTime PublishedOn { get; set; }
    
    public IEnumerable<string> Domains { get; set; }
}