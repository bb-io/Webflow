using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Response;

public class PageResponse
{
    [Display("Site ID")]
    public string SiteId { get; set; }
    
    [Display("Page ID")]
    public string PageId { get; set; }
    
    [Display("Page title")]
    public string PageTitle { get; set; }

    [Display("Published path")]
    public string PublishedPath { get; set; }
}