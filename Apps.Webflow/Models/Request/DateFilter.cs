using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request;

public class DateFilter
{
    [Display("Created after (for collection items and pages)")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before (for collection items and pages)")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Last updated after (for collection items and pages)")]
    public DateTime? LastUpdatedAfter { get; set; }

    [Display("Last updated before (for collection items and pages)")]
    public DateTime? LastUpdatedBefore { get; set; }
}
