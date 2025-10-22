using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Response.Pages;

public class PageResponse
{
    [Display("Site ID")]
    public string? SiteId { get; set; }

    [Display("Page ID")]
    public string Id { get; set; }

    [Display("Page title")]
    public string Title { get; set; } = string.Empty;

    [Display("Slug")]
    public string? Slug { get; set; }

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; }

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; }

    [Display("Locale ID")]
    [JsonProperty("localeId")]
    public string? LocaleId { get; set; }

    [Display("Published path")]
    public string? PublishedPath { get; set; }

    [Display("Archived")]
    public bool? Archived { get; set; }

    [Display("Is draft")]
    public bool? Draft { get; set; }
}
