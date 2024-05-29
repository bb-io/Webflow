using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Response;

public class PageUpdatedResponse : PageResponse
{
    [Display("Last updated")] public DateTime LastUpdated { get; set; }

}