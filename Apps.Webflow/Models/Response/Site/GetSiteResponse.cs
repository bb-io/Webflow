using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Response.Site;

public class GetSiteResponse(SiteEntity entity)
{
    [Display("Site ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Display name")]
    public string? DisplayName { get; set; } = entity.DisplayName;

    [Display("Last published")]
    public DateTime? LastPublished { get; set; } = entity.LastPublished;

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; } = entity.LastUpdated;

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; } = entity.CreatedOn;

    [Display("Primary locale")]
    public string? PrimaryLocale { get; set; } = entity.Locales?.Primary?.Tag;

    [Display("Secondary locales")]
    public IEnumerable<string>? SecondaryLocales { get; set; } = entity.Locales?.Secondary?.Select(x => x.Tag);

    [Display("Custom domains")]
    public IEnumerable<string>? CustomDomains { get; set; } = entity.CustomDomains?.Select(x => x.Url);
}
