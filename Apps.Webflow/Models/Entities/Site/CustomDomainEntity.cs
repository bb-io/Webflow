using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities.Site;

public class CustomDomainEntity
{
    [Display("Custom domain ID")]
    public string Id { get; set; }

    [Display("URL")]
    public string Url { get; set; }

    [Display("Last published")]
    public DateTime LastPublished { get; set; }
}
