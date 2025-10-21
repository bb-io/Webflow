using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Webflow.Models.Response.Pages;

public class GetPageAsHtmlResponse(FileReference content, PageResponse? metadata)
{
    public FileReference Content { get; set; } = content;

    [Display("Site ID")]
    public string? SiteId { get; set; } = metadata?.SiteId;

    [Display("Title")]
    public string? Title { get; set; } = metadata?.Title;

    [Display("Slug")]
    public string? Slug { get; set; } = metadata?.Slug;

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; } = metadata?.CreatedOn;

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; } = metadata?.LastUpdated;

    [Display("Is archived")]
    public bool? Archived { get; set; } = metadata?.Archived;

    [Display("Is draft")]
    public bool? Draft { get; set; } = metadata?.Draft;

    [Display("Locale ID")]
    public string? LocaleId { get; set; } = metadata?.LocaleId;

    [Display("Published path")]
    public string? PublishedPath { get; set; } = metadata?.PublishedPath;
}
