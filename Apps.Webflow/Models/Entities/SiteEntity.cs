using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities;

public class SiteEntity
{
    [Display("Site ID")]
    public string Id { get; set; }

    [Display("Display name")]
    public string DisplayName { get; set; } = string.Empty;

    [Display("Last published")]
    public DateTime? LastPublished { get; set; }

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; }

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; }

    [Display("Locales")]
    public SiteLocales? Locales { get; set; }

    [Display("Custom domains")]
    public IEnumerable<CustomDomainEntity>? CustomDomains { get; set; }
}