using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Webflow.Models.Response.Pages;

public class GetPageAsHtmlResponse(FileReference content, PageMetadataEntity? metadata)
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
    public bool? IsArchived { get; set; } = metadata?.IsArchived;

    [Display("Is draft")]
    public bool? IsDraft { get; set; } = metadata?.IsDraft;

    [Display("Locale ID")]
    public string? LocaleId { get; set; } = metadata?.LocaleId;

    [Display("Published path")]
    public string? PublishedPath { get; set; } = metadata?.PublishedPath;
}
