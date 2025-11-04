using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Request;

public class PageWebhookRequest
{
    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Published path contains")]
    public string? PublishedPathContains { get; set; }
}
