using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Response;

public class PageCreatedResponse : PageResponse
{
    [Display("Created on")] public DateTime CreatedOn { get; set; }
}