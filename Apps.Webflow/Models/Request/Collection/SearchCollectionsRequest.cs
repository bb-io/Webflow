using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Collection;

public class SearchCollectionsRequest
{
    [Display("Display name contains")]
    public string? DisplayNameContains { get; set; }

    [Display("Slug contains")]
    public string? SlugContains { get; set; }

    [Display("Singular name contains")]
    public string? SingluralNameContains { get; set; }
}
