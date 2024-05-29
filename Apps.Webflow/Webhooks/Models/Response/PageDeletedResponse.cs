using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Webhooks.Models.Response;

public class PageDeletedResponse : PageResponse
{
    [Display("Deleted on")] public DateTime DeletedOn { get; set; }
}