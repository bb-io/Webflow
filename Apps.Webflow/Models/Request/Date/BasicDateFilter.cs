using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Date;

public class BasicDateFilter : IDateFilter
{
    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Last updated after")]
    public DateTime? LastUpdatedAfter { get; set; }

    [Display("Last updated before")]
    public DateTime? LastUpdatedBefore { get; set; }
}
